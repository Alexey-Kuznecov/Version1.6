using System;

namespace UnityCommander.Core
{
    public class DirectoryChangeNotifier : IDirectoryChangeNotifier
    {
        public event Action<string> DirectoryChanged;

        public void NotifyChanged(string path)
        {
            DirectoryChanged?.Invoke(path);
        }
    }
}
