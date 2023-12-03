
using System;
using System.IO;

namespace UnityCommander.Core.IO.Operations
{
    /// <summary>
    /// The copy info.
    /// </summary>
    public class CopyInfo
    {
        /// <summary>
        /// Gets or sets the name file.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the source directory.
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// Gets or sets the target directory.
        /// </summary>
        public string Destination { get; set; }

        /// <summary>
        /// Gets or sets the target directory.
        /// </summary>
        public FileInfo FileInfo { get; set; }

        /// <summary>
        /// Gets or sets the file length in bytes.
        /// </summary>
        public long Length { get; set; }

        /// <summary>
        /// Gets or sets the average file copy rate per second.
        /// </summary>
        public double AverageSpeed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the time remaining for copying the file.
        /// </summary>
        public TimeSpan TimeLeft { get; set; }

        /// <summary>
        /// Gets or sets the value indicating the time remaining for copying all files.
        /// </summary>
        public TimeSpan TotalTimeLeft { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the percentage of file copy progress.
        /// </summary>
        public double Percentage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the percentage of files copy progress.
        /// </summary>
        public double TotalPercentage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the copied bytes.
        /// </summary>
        public double ByteDone { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the copied bytes.
        /// </summary>
        public double TotalBytes { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates how many bytes have been copied.
        /// </summary>
        public double TotalByteDone { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates the current file size.
        /// </summary>
        public double CurrentFileSize { get; set; }

        /// <summary>
        /// Gets or sets the value indicating total size of files.
        /// </summary>
        public double TotalFileSize { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether there are copy errors.
        /// </summary>
        public bool Skipped { get; set; }

        public CopyDialogSkipReplaceStatus DialogSkipReplaceStatus { get; set; }

#region Log Properties

        public long CurrentBytesTransferred { get; set; }

        public long TotalBytesTransferred { get; set; }

#endregion
    }
}
