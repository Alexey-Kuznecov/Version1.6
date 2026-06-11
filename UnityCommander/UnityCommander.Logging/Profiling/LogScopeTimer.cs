
using System.Diagnostics;
using UnityCommander.Logging.Configuration;
using UnityCommander.Logging.Core;

namespace UnityCommander.Logging.Profiling
{
    public sealed class LogScopeTimer : IDisposable
    {
        private readonly LogHub _hub;
        private readonly LogScope _scope;
        private readonly string _name;
        private readonly Stopwatch _sw;

        public LogScopeTimer(LogHub hub, LogScope scope, string name)
        {
            _hub = hub;
            _scope = scope;
            _name = name;
            _sw = Stopwatch.StartNew();

            _hub.Publish(new LogEntry
            {
                Scope = scope.Value,
                Category = _name,
                Level = LogLevel.Profile,
                Message = $"Start {_name}"
            });
        }

        public void Dispose()
        {
            _sw.Stop();

            _hub.Publish(new LogEntry
            {
                Scope = _scope.Value,
                Category = _name,
                Level = LogLevel.Profile,
                Message = $"{_name} finished",
                DurationMs = _sw.Elapsed.TotalMilliseconds
            });
        }
    }
}
