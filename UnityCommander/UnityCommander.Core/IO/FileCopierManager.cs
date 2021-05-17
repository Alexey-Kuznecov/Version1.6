
namespace UnityCommander.Core.IO
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// The manager.
    /// </summary>
    public class FileCopierManager : ManagerBase
    {
        /// <summary>
        /// The file copier.
        /// </summary>
        private FileCopier fileCopier;

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
        public Action<FileCopier.Parameters> CopyFileReport { get; set; }

        /// <summary>
        /// Gets or sets the action to processing copy file results.
        /// </summary>
        public Action<FileCopier.Parameters> CopyFileResult { get; set; }

        /// <summary>
        /// Gets or sets the action to processing copy file results.
        /// </summary>
        public Action<FileCopier.Parameters> CopyFileFinish { get; set; }

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

                using (this.fileCopier = new FileCopier())
                {
                    this.fileCopier.CopyProgressReport += this.FileCopier_CopyProgressReport;
                    this.fileCopier.CalculateTotalFilesSize(this.source);
                    this.fileCopier.GetSpeedTimer.Start();
                    this.fileCopier.GetElapsedTimer.Start();

                    foreach (var oldDir in Directory.GetDirectories(this.source, "*", SearchOption.AllDirectories))
                    {
                        var newDir = oldDir.Replace(this.source, this.target);
                        Directory.CreateDirectory(newDir);
                        this.fileCopier.CopyFiles(oldDir, newDir);
                        this.CopyFileResult?.Invoke(this.fileCopier.GetParameters);
                    }

                    // Also copy files in the root directory of the source.
                    if (Directory.GetFiles(this.source).Length != 0)
                    {
                        this.fileCopier.CopyFiles(this.source, this.target);
                    }
                }

                this.CopyFileFinish?.Invoke(this.fileCopier.GetParameters);
            });
        }

        /// <summary>
        /// The pause.
        /// </summary>
        public void Pause()
        {
            this.fileCopier.ChangeCopyStatus(FileCopier.CopyBehaviors.Pause);
        }

        /// <summary>
        /// The resume.
        /// </summary>
        public void Resume()
        {
            this.fileCopier.ChangeCopyStatus(FileCopier.CopyBehaviors.Resume);
        }

        /// <summary>
        /// The cancel.
        /// </summary>
        public void Cancel()
        {
            this.fileCopier.ChangeCopyStatus(FileCopier.CopyBehaviors.Cancel);
            Directory.Delete(this.target, true);
        }

        /// <summary>
        /// The file copier_ copy progress report.
        /// </summary>
        /// <param name="sender"> The sender. </param>
        /// <param name="e"> Expected report data about copy file. </param>
        private void FileCopier_CopyProgressReport(object sender, FileCopier.Parameters e)
        {
            this.CopyFileReport?.Invoke(e);
        }
    }
}
