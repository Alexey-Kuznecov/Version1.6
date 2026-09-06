
using System;
using UnityCommander.CLI.Infrastructure;

namespace UnityCommander.Modules.BottomPanel.Console
{
    internal sealed class ConsoleActivity : IConsoleActivity
    {
        private readonly InternalConsoleOutput _output;

        public ConsoleActivity(InternalConsoleOutput output)
        {
            _output = output;
        }

        public void Update(Action<IConsoleActivityState> update)
        {
            _output.UpdateActivity(update);
        }

        public void Dispose()
        {
            _output.CompleteActivity();
        }
    }
}
