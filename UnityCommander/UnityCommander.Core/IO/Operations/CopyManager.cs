#define Nlog

using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using UnityCommander.Core.Helper;

namespace UnityCommander.Core.IO.Operations
{
    public class CopyManager : ManagerBase
    {
#if (Nlog)
        /// <summary>
        /// The reference the current log event manager.
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
#endif

        /// <summary>
        /// The file copier.
        /// </summary>
        private CopyFiles copyFile;

        /// <summary>
        /// The source path to the directory.
        /// </summary>
        private string source;

        /// <summary>
        /// The target path to the directory.
        /// </summary>
        private string target;

        /// <summary>
        /// The target path to the directory.
        /// </summary>
        private static CancellationTokenSource cancellationTokenSource;

        /// <summary>
        /// Gets or sets copy progress report..
        /// </summary>
        public Action<CopyInfo> CopyFileReport { get; set; }

        /// <summary>
        /// Gets or sets the action to processing copy file results.
        /// </summary>
        public Action<CopyInfo> CopyFileResult { get; set; }

        /// <summary>
        /// Gets or sets the action to processing copy file results.
        /// </summary>
        public Action<CopyInfo> CopySkipReplace { get; set; }

        /// <summary>
        /// Gets or sets the action to processing copy file results.
        /// </summary>
        public Action CopyFileFinish { get; set; }

        /// <summary>
        /// The copy progress report.
        /// </summary>
        public event EventHandler FileAlreadyExistsEvent;

        /// <summary>
        /// This method is created.
        /// </summary>
        /// <param name="sourcePath"> The <c>source</c> path to the directory. </param>
        /// <param name="targetPath"> The <c>target</c> path to the directory. </param>
        public void Copy(string sourcePath, string targetPath)
        {
            this.source = sourcePath;
            this.target = targetPath;
            cancellationTokenSource = new CancellationTokenSource();
            Task.Factory.StartNew(() => CopyTask(cancellationTokenSource.Token), cancellationTokenSource.Token);
        }

        public async void CopyTask(CancellationToken cancellationToken)
        {
            using (this.copyFile = new CopyFiles())
            {
                cancellationToken.Register(CollectionTokenCancel);
                this.copyFile.FileAlreadyExistsEvent += FileCopier_CopySkipReplace;
                //if (File.Exists(source.Replace(new FileInfo(source).Directory.FullName, target)))
                //{
                //    await Task.Delay(100, cancellationToken);
                //}

                await Task.Run(() =>
                {
                    if (File.Exists(source.Replace(new FileInfo(source).Directory.FullName, target)))
                        cancellationTokenSource.Cancel();

                    this.copyFile.CopyReportEvent += this.FileCopier_CopyReportEvent;

                    if (File.Exists(this.source))
                        this.copyFile.Copy(this.source, this.target);

                    if (Directory.Exists(this.source))
                        this.copyFile.DeepCopy(this.source, this.target);
                    
                    this.copyFile.CopyReportEvent -= this.FileCopier_CopyReportEvent;
                    this.CopyFileFinish?.Invoke();

                }, cancellationToken);
            }
        }

        private void CollectionTokenCancel()
        {
        }


        private void FileCopier_CopySkipReplace(object sender, EventArgs e)
        {
            var copyArgs = (CopyReportEventArg)e;
            this.CopySkipReplace?.Invoke(copyArgs.Info);
        }

        /// <summary>
        /// The raise event.
        /// </summary>
        /// <param name="info">
        /// The info.
        /// </param>
        private void RaiseFileAlreadyExistsEvent(CopyInfo info)
        {
            if (this.FileAlreadyExistsEvent != null)
            {
                this.FileAlreadyExistsEvent.Invoke(this, new CopyReportEventArg(info));
            }
        }

        private void FileCopier_CopyReportEvent(object sender, EventArgs e)
        {
            var copyArgs = (CopyReportEventArg)e;
            this.CopyFileReport?.Invoke(copyArgs.Info);
#if (Nlog)
            Logger.Info(string.Format("{0} | {1} | {2} | {3} | {4}",
                copyArgs.Info.Name,
                ConverterBytes.AutoConvertFormatBytes((decimal)copyArgs.Info.CurrentFileSize),
                ConverterBytes.AutoConvertFormatBytes((decimal)copyArgs.Info.AverageSpeed),
                ConverterBytes.AutoConvertFormatBytes((decimal)copyArgs.Info.CurrentBytesTransferred),
                ConverterBytes.AutoConvertFormatBytes((decimal)copyArgs.Info.TotalBytesTransferred)));
#endif
        }

        /// <summary>
        /// The pause.
        /// </summary>
        public void Pause()
        {
            this.copyFile.ChangeCopyStatus(CopyBehaviors.Pause);
        }

        /// <summary>
        /// The resume.
        /// </summary>
        public void Resume()
        {
            this.copyFile.ChangeCopyStatus(CopyBehaviors.Resume);
        }

        /// <summary>
        /// The cancel.
        /// </summary>
        public void Cancel()
        {
            this.copyFile.ChangeCopyStatus(CopyBehaviors.Cancel);
            Directory.Delete(this.target, true);
        }
    }
}
