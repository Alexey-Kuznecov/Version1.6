#define Nlog

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NLog;

namespace UnityCommander.Core.IO.Operations
{
    public class CopyManager : ManagerBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private CopyFiles copyFile;
        private string source;
        private string targetRoot;

        private static CancellationTokenSource cancellationTokenSource;

        public bool CopyOnlyFolderContent { get; set; } = true;

        // События для внешнего мира (UI, панели и т.д.)
        public event Action<CopyInfo> FileStarted;
        public event Action<CopyInfo> FileCompleted;
        public event Action<string> DirectoryCreated;
        public event Action<CopyInfo> CopySkipped;
        public event Action CopyFileFinish;
        public event Action<CopyInfo> CopyFileReport;
        public event Action<CopyInfo> CopyFileResult;

        // Подписки
        private void SubscribeEvents()
        {
            copyFile.FileStarted += info => FileStarted?.Invoke(info);
            copyFile.FileCompleted += info => FileCompleted?.Invoke(info);
            copyFile.DirectoryCreated += dir => DirectoryCreated?.Invoke(dir);
            copyFile.FileAlreadyExistsEvent += (sender, e) =>
            {
                var args = (CopyReportEventArg)e;
                CopySkipped?.Invoke(args.Info);
            };
            copyFile.CopyReportEvent += FileCopier_CopyReportEvent;
        }

        // Основной метод копирования
        public void Copy(string sourcePath, string targetPath)
        {
            this.source = sourcePath;
            var src = new DirectoryInfo(sourcePath);

            if (!this.CopyOnlyFolderContent && src.Exists)
            {
                // Формируем корень назначения, добавляя имя папки источника
                this.targetRoot = Path.Combine(targetPath, src.Name);
                Directory.CreateDirectory(this.targetRoot);
            }
            else
            {
                this.targetRoot = targetPath;
            }
            
            // Создаём один экземпляр CopyFiles
            this.copyFile = new CopyFiles
            {
                SourceRoot = sourcePath,
                TargetRoot = targetPath
            };

            // Подписываем события
            SubscribeEvents();

            // Создаём токен отмены
            cancellationTokenSource = new CancellationTokenSource();

            // Запускаем копирование в отдельном таске
            Task.Run(() => CopyTask(cancellationTokenSource.Token), cancellationTokenSource.Token);
        }

        private void CopyTask(CancellationToken cancellationToken)
        {
            cancellationToken.Register(() => copyFile.ChangeCopyStatus(CopyBehaviors.Cancel));

            // Если это файл
            if (File.Exists(source))
            {
                copyFile.Copy(source, targetRoot);
            }
            // Если это папка
            else if (Directory.Exists(source))
            {
                copyFile.DeepCopy(source, targetRoot);
            }

            // Завершение копирования
            CopyFileFinish?.Invoke();

            // Отписка от событий после окончания
            copyFile.FileStarted -= info => FileStarted?.Invoke(info);
            copyFile.FileCompleted -= info => FileCompleted?.Invoke(info);
            copyFile.DirectoryCreated -= dir => DirectoryCreated?.Invoke(dir);
            copyFile.FileAlreadyExistsEvent -= (sender, e) =>
            {
                var args = (CopyReportEventArg)e;
                CopySkipped?.Invoke(args.Info);
            };

            copyFile.CopyReportEvent -= FileCopier_CopyReportEvent;
        }

        private void FileCopier_CopyReportEvent(object sender, EventArgs e)
        {
            var copyArgs = (CopyReportEventArg)e;
            this.CopyFileReport?.Invoke(copyArgs.Info);
        }

        public void Pause() => copyFile.ChangeCopyStatus(CopyBehaviors.Pause);
        public void Resume() => copyFile.ChangeCopyStatus(CopyBehaviors.Resume);
        public void Cancel()
        {
            copyFile.ChangeCopyStatus(CopyBehaviors.Cancel);
            if (Directory.Exists(targetRoot))
                Directory.Delete(targetRoot, true);
        }
    }
}
