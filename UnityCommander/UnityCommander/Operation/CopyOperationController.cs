using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityCommander.Core;
using UnityCommander.Core.IO;
using UnityCommander.Core.IO.Operations;

namespace UnityCommander.Operation
{
    public class CopyOperationController : IDisposable
    {
        private readonly IDirectoryChangeNotifier notifier;
        private readonly CopyManager copyManager;
        private readonly CopyProgressCalculator progressCalculator;
        private readonly CopyConflictResolver conflictResolver;
        private readonly CopyReportCollector reportCollector;

        // События для VM
        public event Action<ProgressModel> ProgressChanged;
        public event Action<CopyInfoModel> FileCopied;
        public event Action<CopyInfoModel> FileSkipped;
        public event Action Completed;

        // агрегатор состояния при множественных источниках
        private long _totalBytesAll = 0;
        private long _accumulatedBytesFromPreviousSources = 0;
        private long _currentSourceTotalBytes = 0; // for info if needed
        private int _completedSources = 0;
        private int _totalSources = 0;
        public CopyOperationController(
            IDirectoryChangeNotifier notifier,
            CopyManager copyManager,
            CopyProgressCalculator progressCalculator,
            CopyConflictResolver conflictResolver,
            CopyReportCollector reportCollector)
        {
            this.notifier = notifier ?? throw new ArgumentNullException(nameof(notifier));
            this.copyManager = copyManager;
            this.progressCalculator = progressCalculator;
            this.conflictResolver = conflictResolver;
            this.reportCollector = reportCollector;

            // Подписка на события CopyManager
            this.copyManager.CopyFileReport += OnCopyFileReport;
            this.copyManager.CopyFileFinish += OnCopyFileFinish;
            this.copyManager.FileCompleted += OnCopyFileCompleted;
            this.copyManager.FileStarted += OnFileStarted;
            this.copyManager.DirectoryCreated += OnDirectoryCreated;
            this.copyManager.CopySkipped += OnCopySkipReplace;
        }

        private void OnCopyFileCompleted(CopyInfo info)
        {
            this.notifier.NotifyChanged(info.Root);
        }

        // Команды управления копированием
        public void Pause() => copyManager.Pause();
        public void Resume() => copyManager.Resume();
        public void Cancel() => copyManager.Cancel();

        // Запуск копирования
        public void StartCopy(string source, string destination)
        {
            reportCollector.Clear();
            copyManager.Copy(source, destination);
        }

        public async Task StartCopyManyAsync(IEnumerable<string> sources, string destinationRoot)
        {
            if (sources == null) return;
            var sourcesList = sources.ToList();

            _totalSources = sourcesList.Count;


            // 1) вычисляем общий размер всех источников (можно быть дорогая операция)
            _totalBytesAll = 0;
            foreach (var s in sources)
            {
                _totalBytesAll += GetDirectoryOrFileSizeSafe(s);
            }

            _accumulatedBytesFromPreviousSources = 0;

            // последовательно запускаем копирование каждого источника
            foreach (var s in sources)
            {
                // Для текущего источника задаём текущий root (если нужно)
                // если целевой root содержит папку-имя источника
                var srcInfo = new DirectoryInfo(s);
                string destForThisSource;
                if (!copyManager.CopyOnlyFolderContent && srcInfo.Exists)
                    destForThisSource = Path.Combine(destinationRoot, srcInfo.Name);
                else
                    destForThisSource = destinationRoot;

                Directory.CreateDirectory(destForThisSource);

                // запомним размер текущего источника
                _currentSourceTotalBytes = GetDirectoryOrFileSizeSafe(s);

                // Запустим копирование и дождёмся завершения (CopyAsync использует внутренний TCS)
                await copyManager.CopyAsync(s, destForThisSource);

                // Когда source полностью скопирован, увеличиваем накопитель
                _accumulatedBytesFromPreviousSources += _currentSourceTotalBytes;
                // при переходе к следующему источнику текущий прогресс будет основываться на новом accumulated
            }

            // всё завершено
            Completed?.Invoke();
        }

        // События CopyManager
        private void OnCopyFileReport(CopyInfo info)
        {
            // Глобальные байты done с накоплением из предыдущих файлов
            long currentSourceBytesDone = (long)info.TotalByteDone;
            long overallBytesDone = _accumulatedBytesFromPreviousSources + currentSourceBytesDone;

            // Формирование нового CopyInfo для глобального процесса
            double percent = _totalBytesAll > 0
                         ? (double)overallBytesDone / _totalBytesAll * 100.0
                         : info.TotalPercentage;

            info.TotalBytes = _totalBytesAll;
            info.TotalByteDone = overallBytesDone;
            info.AverageSpeed = info.AverageSpeed;       // не ломаем скорость
            info.TotalTimeLeft = info.TotalTimeLeft;      // не ломаем оставшееся время
            info.TotalPercentage = percent;

            // Используем калькулятор
            var progress = progressCalculator.Calculate(info);

            // Отдаем в UI
            ProgressChanged?.Invoke(progress);
        }

        private void OnCopyFileFinish()
        {
            // TODO: Решить проблему с множественным вызовом
            _completedSources++;
            if (_completedSources == _totalSources)
            {
                Completed?.Invoke();
            }
        }

        private void OnFileStarted(CopyInfo copyInfo)
        {
            // this.notifier.NotifyChanged(changedPath);
        }

        private void OnDirectoryCreated(string dir)
        {
            this.notifier.NotifyChanged(dir);
        }

        private void OnCopySkipReplace(CopyInfo info)
        {
            var action = conflictResolver.ResolveConflict(info);
            switch (action)
            {
                case CopyConflictAction.Skip:
                    info.Skipped = true;
                    FileSkipped?.Invoke(reportCollector.AddSkipped(info));
                    break;
                case CopyConflictAction.Replace:
                    copyManager.Resume();
                    FileCopied?.Invoke(reportCollector.AddCopied(info));
                    break;
                case CopyConflictAction.Cancel:
                    Cancel();
                    break;
                case CopyConflictAction.ReplaceAll:
                    conflictResolver.SetReplaceAll();
                    copyManager.Resume();
                    FileCopied?.Invoke(reportCollector.AddCopied(info));
                    break;
                case CopyConflictAction.SkipAll:
                    conflictResolver.SetSkipAll();
                    info.Skipped = true;
                    FileSkipped?.Invoke(reportCollector.AddSkipped(info));
                    break;
            }
        }

        public void Dispose()
        {
            copyManager.CopyFileReport -= OnCopyFileReport;
            copyManager.CopyFileFinish -= OnCopyFileFinish;
            copyManager.CopySkipped -= OnCopySkipReplace;
        }

        private long GetDirectoryOrFileSizeSafe(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    return new FileInfo(path).Length;
                }
                else if (Directory.Exists(path))
                {
                    long total = 0;
                    foreach (var f in Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories))
                    {
                        try { total += new FileInfo(f).Length; } catch { }
                    }
                    return total;
                }
            }
            catch { /* ignore */ }
            return 0;

        }
    }
}
