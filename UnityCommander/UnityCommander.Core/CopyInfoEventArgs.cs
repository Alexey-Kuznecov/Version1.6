using System;

namespace UnityCommander.Core
{
    /// <summary>
    /// Class CopyInfoEventArgs.
    /// </summary>
    public class CopyInfoEventArgs : EventArgs
    {
        public CopyInfoModel CopyInfo { get; private set; }
        public CopyInfoEventArgs(CopyInfoModel copyInfo)
        {
            this.CopyInfo = copyInfo;
        }
    }
}
