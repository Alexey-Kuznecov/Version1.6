using Prism.Events;
using System;
using UnityCommander.CLI.Core;

namespace UnityCommander.Modules.BottomPanel
{
    public class InternalConsoleOutput : IConsoleOutput
    {
        private readonly IEventAggregator _ea;

        public InternalConsoleOutput(IEventAggregator ea)
        {
            _ea = ea;
        }

        public void Write(string text)
        {
            _ea.GetEvent<ConsoleWriteEvent>().Publish(text);
        }

        public void WriteLine(string text)
        {
            _ea.GetEvent<ConsoleWriteEvent>().Publish(text + Environment.NewLine);
        }

        public void WriteError(string message)
        {
            WriteLine("[ERROR] " + message);
        }

        public void WriteWarning(string message)
        {
            WriteLine("[WARNING] " + message);
        }

        public void WriteSuccess(string message)
        {
            WriteLine("[OK] " + message);
        }

        public void Clear()
        {
            _ea.GetEvent<ConsoleClearEvent>().Publish();
        }
    }
}
