using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityCommander.Core.IO;
using UnityCommander.Core.IO.Operations;

namespace UnityCommander.Operation
{
    public class CopyOperationController : IDisposable
    {
        private readonly CopyManager copyManager;
        private readonly CopyProgressCalculator progressCalculator;
        private readonly CopyConflictResolver conflictResolver;
        private readonly CopyReportCollector reportCollector;

        // События для VM
        public event Action<ProgressModel> ProgressChanged;
        public event Action<CopyInfoModel> FileCopied;
        public event Action<CopyInfoModel> FileSkipped;
        public event Action Completed;

        public CopyOperationController(
            CopyManager copyManager,
            CopyProgressCalculator progressCalculator,
            CopyConflictResolver conflictResolver,
            CopyReportCollector reportCollector)
        {
            this.copyManager = copyManager;
            this.progressCalculator = progressCalculator;
            this.conflictResolver = conflictResolver;
            this.reportCollector = reportCollector;

            // Подписка на события CopyManager
            this.copyManager.CopyFileReport += OnCopyFileReport;
            this.copyManager.CopyFileFinish += OnCopyFileFinish;
            this.copyManager.CopySkipReplace += OnCopySkipReplace;
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

        // События CopyManager
        private void OnCopyFileReport(CopyInfo info)
        {
            var progress = progressCalculator.Calculate(info);
            ProgressChanged?.Invoke(progress);
        }

        private void OnCopyFileFinish()
        {
            Completed?.Invoke();
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
            copyManager.CopySkipReplace -= OnCopySkipReplace;
        }
    }
}
