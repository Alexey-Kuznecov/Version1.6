
using System.Collections.Generic;
using UnityCommander.Logging.Abstractions;

namespace UnityCommander.Services
{
    public sealed class LoggingSinkService
    {
        private readonly LogHub _hub;
        private readonly List<ILogSink> _sinks = new();

        public LoggingSinkService(LogHub hub, IEnumerable<ILogSink> sinks)
        {
            _hub = hub;

            foreach (var sink in sinks)
            {
                _hub.LogReceived += sink.Emit;
                _sinks.Add(sink);
            }
        }
    }
}
