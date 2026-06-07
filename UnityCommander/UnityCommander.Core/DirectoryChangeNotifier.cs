using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

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
