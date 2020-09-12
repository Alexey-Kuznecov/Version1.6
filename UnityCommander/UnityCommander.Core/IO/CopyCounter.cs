
namespace UnityCommander.Core.IO
{
    using System;
    using System.Diagnostics;

    using UnityCommander.Core.Processes;

    /// <summary>
    /// The copy counter.
    /// </summary>
    public class CopyCounter
    {
        /// <summary>
        /// The total bytes remaining to copy.
        /// </summary>
        private decimal bytesRemain;

        /// <summary>
        /// The total bytes copied.
        /// </summary>
        private decimal bytesDone;

        /// <summary>
        /// Elapsed time since copying started.
        /// </summary>
        private DateTime timeElapsed;

        /// <summary>
        /// The total bytes should be copy.
        /// </summary>
        private decimal totalBytes;

        /// <summary>
        /// Time taken to copy one byte in seconds.
        /// </summary>
        private int speed;

        /// <summary>
        /// Initializes a new instance of the <see cref="CopyCounter"/> class.
        /// </summary>
        public CopyCounter()
        {
            Process[] processes = Process.GetProcessesByName("Explorer");
            ProcessCounter counter = new ProcessCounter(processes[0]);
        }
    }
}
