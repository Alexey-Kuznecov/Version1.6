
using Prism.Events;
using System;
using UnityCommander.Logging.Contracts;
using UnityCommander.Logging.Core;
using UnityCommander.Modules.BottomPanel;

namespace UnityCommander.Sinks
{
    public sealed class InternalConsoleSink : ILogSink
    {
        private readonly IEventAggregator _ea;

        public InternalConsoleSink(IEventAggregator ea)
        {
            _ea = ea;
        }

        public void Emit(LogEntry entry)
        {
            var text = $"[{entry.Level}] {entry.Message}";
            _ea.GetEvent<ConsoleWriteEvent>().Publish(text + Environment.NewLine);
        }
    }
}
