using System;

namespace UnityCommander.Core.IO
{
    /// <summary>
    /// Class is an event representative <see cref="FileDublicator.CopyingEvent"/>.
    /// </summary>
    public class CopyInfoEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or set the data on the copying file.
        /// </summary>
        public CopyInfoModel ProgressBarInfo { get; private set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="CopyInfoEventArgs"/> class.
        /// </summary>
        /// <param name="copyInfo"> Expected for file data </param>
        public CopyInfoEventArgs(CopyInfoModel copyInfo)
        {
            this.ProgressBarInfo = copyInfo;
        }
    }
}
