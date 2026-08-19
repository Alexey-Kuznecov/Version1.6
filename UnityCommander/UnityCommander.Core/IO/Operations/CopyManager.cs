#define Nlog

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityCommander.Abstractions.IO;

namespace UnityCommander.Core.IO.Operations
{
    public class CopyManager : ICopyManager
    {
        private readonly CopyOperation _operation;

        private CopyFiles copyFile;
        private string source;
        private string targetRoot;

        private static CancellationTokenSource cancellationTokenSource;
        private TaskCompletionSource<bool> _currentTcs;

        public bool CopyOnlyFolderContent { get; set; } = false;

        public Guid Id => _operation.Id;

        public CopyOperation Operation => _operation;

        public event Action<CopyInfo> FileStarted;
        public event Action<CopyInfo> FileCompleted;
        public event Action<string> DirectoryCreated;
        public event Action<CopyInfo> CopySkipped;
        public event Action CopyFileFinish;
        public event Action<CopyInfo> CopyFileReport;
        public event Action<CopyInfo> CopyFileResult;

        public CopyManager(CopyOperation operation)
        {
            _operation = operation;
        }

        public void Pause() => copyFile.ChangeCopyStatus(CopyBehaviors.Pause);
        public void Resume() => copyFile.ChangeCopyStatus(CopyBehaviors.Resume);
        public void Cancel()
        {
            copyFile.ChangeCopyStatus(CopyBehaviors.Cancel);
            cancellationTokenSource.Cancel();
        }

        public Task CopyAsync(OperationContext ctx, string sourcePath, string targetPath)
        {
            _currentTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

            Copy(ctx, sourcePath, targetPath);

            return _currentTcs.Task;
        }

        private void Copy(OperationContext ctx, string sourcePath, string targetPath)
        {
            this.source = sourcePath;
            var src = new DirectoryInfo(sourcePath);

            targetRoot = targetPath;
            Directory.CreateDirectory(targetRoot);

            this.copyFile = new CopyFiles(ctx)
            {
                SourceRoot = sourcePath,
                TargetRoot = targetPath
            };

            SubscribeEvents();

            cancellationTokenSource = new CancellationTokenSource();

            Task.Run(() => CopyTask(cancellationTokenSource.Token), cancellationTokenSource.Token);
        }

        private void CopyTask(CancellationToken cancellationToken)
        {
            cancellationToken.Register(() =>
                copyFile.ChangeCopyStatus(CopyBehaviors.Cancel));

            try
            {
                if (File.Exists(source))
                {
                    copyFile.Copy(source, targetRoot);
                }
                else if (Directory.Exists(source))
                {
                    copyFile.DeepCopy(source, targetRoot);
                }

                CopyFileFinish?.Invoke();

                _currentTcs?.TrySetResult(true);
            }
            catch (OperationCanceledException)
            {
                _currentTcs?.TrySetCanceled(cancellationToken);
                throw;
            }
            catch (Exception ex)
            {
                _currentTcs?.TrySetException(ex);
                throw;
            }
            finally
            {
                copyFile.CopyReportEvent -= FileCopier_CopyReportEvent;
            }
        }

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

        private void FileCopier_CopyReportEvent(object sender, EventArgs e)
        {
            var copyArgs = (CopyReportEventArg)e;
            this.CopyFileReport?.Invoke(copyArgs.Info);
        }
    }
}
