using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityCommander.Core.Commands
{
    public class AsyncNavigator : ICommandExecutor
    {
        private readonly Func<object, Task> _asyncAction;
        private readonly object _path;

        public AsyncNavigator(Func<object, Task> asyncAction, object path)
        {
            _asyncAction = asyncAction;
            _path = path;
        }

        public async Task ExecuteAsync()
        {
            await _asyncAction(_path);
        }
    }
}
