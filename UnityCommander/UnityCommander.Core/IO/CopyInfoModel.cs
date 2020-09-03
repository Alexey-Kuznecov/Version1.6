using System;
using System.Collections.Generic;

namespace UnityCommander.Core.IO
{
    public class CopyInfoModel
    {
        /// <summary>
        /// Gets or sets the destination directory.
        /// </summary>
        public string Destination { get; set; }
        /// <summary>
        /// Gets or sets the source directory.
        /// </summary>
        public string Source { get; set; }
        /// <summary>
        /// Gets or sets the approximate time to copy a file.
        /// </summary>
        public DateTime CopyTime { get; set; }
        /// <summary>
        /// Gets or sets the size of the file.
        /// </summary>
        public long FileLength { get; set; }
        /// <summary>
        /// Gets or sets progress bar of the copy file.
        /// </summary>
        public int ProgressBar { get; set; }
        /// <summary>
        /// Gets or sets the method called to cancel the copy operation.
        /// </summary>
        public Action CancelationFlag { get; set; }
        /// <summary>
        /// Gets or sets the files copy error report.
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}
