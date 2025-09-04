using CommandSystem.Core.Abstractions;
using CommandSystem.Core.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UnityCommander.Modules.FilePanel.Commands
{
    public class GetPathCommand : IAsyncCommand
    {
        public string Name => throw new NotImplementedException();

        public string Description => throw new NotImplementedException();

        public bool CanExecute(CommandContext context)
        {
            throw new NotImplementedException();
        }

        public Task ExecuteAsync(CommandContext context)
        {
            throw new NotImplementedException();
        }

        public Task ExecuteAsync(CommandContext context, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
