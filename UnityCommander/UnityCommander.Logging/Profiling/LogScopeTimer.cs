
using System.Diagnostics;
using UnityCommander.Logging.Configuration;
using UnityCommander.Logging.Core;

namespace UnityCommander.Logging.Profiling
{
    public sealed class LogScopeTimer : IDisposable
    {
        private readonly LoggerCore _core;
        private readonly LogScope _scope;
        private readonly string _name;
        private readonly Stopwatch _sw;

        public LogScopeTimer(
            LoggerCore core,
            LogScope scope,
            string name)
        {
            _core = core;
            _scope = scope;
            _name = name;
            _sw = Stopwatch.StartNew();

            _core.Process(new LogEntry
            {
                Scope = scope.Value,
                Category = name,
                Level = LogLevel.Profile,
                Message = $"Start {name}"
            });
        }

        public void Dispose()
        {
            _sw.Stop();

            _core.Process(new LogEntry
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
