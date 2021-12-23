
namespace UnityCommander.Core.IO.Operations
{
    using System;
    using System.Collections.Generic;
    using System.Text;

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

        public double TotalByteDone { get; set; }

        /// <summary>
        /// Gets or sets the value indicating file size.
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
    }
}
