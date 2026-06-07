using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityCommander.Core
{
    public interface IDirectoryChangeNotifier
    {
        event Action<string> DirectoryChanged;
        void NotifyChanged(string path);
    }
}
