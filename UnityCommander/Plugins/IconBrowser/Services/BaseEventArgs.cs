using System;

namespace AIconBrowser.Services
{
    public class BaseEventArgs : EventArgs
    {
        public BaseEventArgs(object param)
        {
            Package = param;
        }
        public object Package { get; }
    }
}
