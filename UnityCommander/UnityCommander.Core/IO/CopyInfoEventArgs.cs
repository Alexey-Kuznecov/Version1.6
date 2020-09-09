
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CopyInfoEventArgs.cs" company="T">
// Copyright (p) Alexey Kuznecov. All right reserved.
// </copyright>
// <summary>
//  The class is a data wrapper for the file copy event.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace UnityCommander.Core.IO
{
    using System;

    /// <summary>
    /// The class is a data wrapper for the file copy event. <see cref="FileDublicator.CopyingEvent"/>.
    /// </summary>
    public class CopyInfoEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CopyInfoEventArgs"/> class.
        /// </summary>
        /// <param name="copyInfo"> Expected for file data </param>
        public CopyInfoEventArgs(CopyInfoModel copyInfo)
        {
            this.ProgressBarInfo = copyInfo;
        }

        /// <summary>
        /// Gets the data on the copying file.
        /// </summary>
        public CopyInfoModel ProgressBarInfo { get; }
    }
}
