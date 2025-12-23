
using System;
using System.Collections.Generic;
using System.Linq;
using UnityCommander.Logging.Abstractions;

namespace UnityCommander.Services
{
    public sealed class Logger : ILogger
    {
        private readonly IReadOnlyList<ILogSink> _sinks;

        public Logger(IEnumerable<ILogSink> sinks)
        {
            _sinks = sinks.ToList();
        }

        public void Log(LogLevel level, string message, Exception? ex = null)
        {
            var entry = new LogEntry
            {
                Timestamp = DateTime.Now,
                Level = level,
                Message = message,
                Exception = ex
            };

            foreach (var sink in _sinks)
                sink.Emit(entry);
        }

        public void Info(string m) => Log(LogLevel.Info, m);
        public void Debug(string m) => Log(LogLevel.Debug, m);
        public void Warning(string m) => Log(LogLevel.Warning, m);
        public void Trace(string m) => Log(LogLevel.Trace, m);
        public void Error(string m, Exception? e = null) => Log(LogLevel.Error, m, e);
        public void Fatal(string m, Exception? e = null) => Log(LogLevel.Fatal, m, e);
    }
}
