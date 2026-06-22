
using System;

namespace UnityCommander.Core
{
    public interface IDirectoryChangeNotifier
    {
        event Action<string> DirectoryChanged;
        void NotifyChanged(string path);
    }
}
