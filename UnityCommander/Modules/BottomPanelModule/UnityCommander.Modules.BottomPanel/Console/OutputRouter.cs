
using System;
using System.Collections.Concurrent;
using UnityCommander.CLI.Core;

namespace UnityCommander.Modules.BottomPanel.Console
{
    public sealed class OutputRouter : IOutputRouter
    {
        private readonly ConcurrentDictionary<Guid, IConsoleOutput> _outputs = new();

        public IConsoleOutput GetOutput(Guid consoleId)
        {
            return _outputs[consoleId];
        }

        public void Register(
            Guid consoleId,
            IConsoleOutput output)
        {
            _outputs[consoleId] = output;
        }

        public void Unregister(Guid consoleId)
        {
            _outputs.TryRemove(consoleId, out _);
        }
    }
}
