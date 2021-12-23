using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace UnityCommander.Core.IO.Operations
{
    public class CopyManager : ManagerBase
    { /// <summary>
      /// The file copier.
      /// </summary>
        private CopyFiles fileCopier;

        /// <summary>
        /// The source path to the directory.
        /// </summary>
        private string source;

        /// <summary>
        /// The target path to the directory.
        /// </summary>
        private string target;

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
        public Action<CopyInfo> CopyFileFinish { get; set; }

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

                using (this.fileCopier = new CopyFiles())
                {
                    this.fileCopier.CopyReportEvent += this.FileCopier_CopyReportEvent;
                    this.fileCopier.Copy(this.source, this.target);

                    this.fileCopier.CopyReportEvent -= this.FileCopier_CopyReportEvent;
                }

                //this.CopyFileFinish?.Invoke(this.fileCopier.GetParameters);
            });
        }

        /// <summary>
        /// The copy file.
        /// </summary>
        /// <param name="sourcePath">
        /// The source path.
        /// </param>
        /// <param name="targetPath">
        /// The target path.
        /// </param>
        public void CopyFile(string sourcePath, string targetPath)
        {
            Task.Factory.StartNew(() =>
            {
                using (this.fileCopier = new CopyFiles())
                {
                    this.source = sourcePath;
                    this.target = targetPath;
                    this.fileCopier.CopyReportEvent += this.FileCopier_CopyReportEvent;
                    this.fileCopier.Copy(this.source, this.target);
                }
            });
        }

        private void FileCopier_CopyReportEvent(object sender, EventArgs e)
        {
            var copyArgs = (CopyReportEventArg)e;
            this.CopyFileReport?.Invoke(copyArgs.Info);
        }

        /// <summary>
        /// The pause.
        /// </summary>
        public void Pause()
        {
            this.fileCopier.ChangeCopyStatus(CopyBehaviors.Pause);
        }

        /// <summary>
        /// The resume.
        /// </summary>
        public void Resume()
        {
            this.fileCopier.ChangeCopyStatus(CopyBehaviors.Resume);
        }

        /// <summary>
        /// The cancel.
        /// </summary>
        public void Cancel()
        {
            this.fileCopier.ChangeCopyStatus(CopyBehaviors.Cancel);
            Directory.Delete(this.target, true);
        }
    }
}
