using System;

namespace IconBrowser.Services
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
