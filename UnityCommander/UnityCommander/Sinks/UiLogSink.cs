using System;
using UnityCommander.Logging.Abstractions;

namespace UnityCommander.Sinks
{
    public sealed class UiLogSink : ILogSink
    {
        private readonly Action<LogEntry> _onLog;

        public UiLogSink(Action<LogEntry> onLog)
        {
            _onLog = onLog;
        }

        public void Emit(LogEntry entry)
        {
            _onLog(entry);
        }
    }
}
