#define Nlog

using System;
using System.IO;
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

        public CopyManager()
        {
        }

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
        public Action CopyFileFinish { get; set; }

        /// <summary>
        /// This method is created.
        /// </summary>
        /// <param name="sourcePath"> The <c>source</c> path to the directory. </param>
        /// <param name="targetPath"> The <c>target</c> path to the directory. </param>
        public void Copy(string sourcePath, string targetPath)
        {
            Task.Factory.StartNew(() =>
            {
                this.source = sourcePath;
                this.target = targetPath;

                using (this.copyFile = new CopyFiles())
                {
                    this.copyFile.CopyReportEvent += this.FileCopier_CopyReportEvent;
                    
                    if (File.Exists(sourcePath))
                    {
                        this.copyFile.Copy(this.source, this.target);
                    }
                    else
                    {
                        this.copyFile.DeepCopy(this.source, this.target);
                    }

                    this.copyFile.CopyReportEvent -= this.FileCopier_CopyReportEvent;
                }

                this.CopyFileFinish?.Invoke();
            });
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
