
namespace UnityCommander.Core.IO.Operations
{
    using System;

    /// <summary>
    /// The copy report event handler.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    public delegate void CopyReportEventHandler(object sender, CopyReportEventArg e);

    /// <summary>
    /// The copy report event arguments.
    /// </summary>
    public class CopyReportEventArg : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CopyReportEventArg"/> class.
        /// </summary>
        /// <param name="info">
        /// The copy Info.
        /// </param>
        public CopyReportEventArg(CopyInfo info)
        {
            this.Info = info;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        public CopyInfo Info { get; }
    }
}
