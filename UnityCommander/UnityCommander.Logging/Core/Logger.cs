using UnityCommander.Logging.Abstractions.Helper;
using UnityCommander.Logging.Configuration;
using UnityCommander.Logging.Contracts;

namespace UnityCommander.Logging.Core
{
    public class Logger : ILogger
    {
        private readonly LoggerCore _core;
        private readonly string _category;
        private readonly string _scope;

        public Logger(LoggerCore core, string category, string scope)
        {
            _core = core;
            _category = category;
            _scope = scope;
        }
        private void LogInternal(
            LogLevel level,
            string message,
            object? payload = null,
            Exception? ex = null,
            Func<bool>? condition = null)
        {
            var obj = payload != null ? ObjectLogFormatter.Format(payload) : null;

            var entry = new LogEntry
            {
                Timestamp = DateTime.Now,
                Level = level,
                Message = message,
                Payload = obj,
                Exception = ex,
                Category = _category,
                Scope = _scope,
                Condition = condition
            };

            _core.Process(entry);
        }

        public void Debug(string message, Func<bool>? condition = null) => LogInternal(LogLevel.Debug, message, condition: condition);
        public void Info(string m) => LogInternal(LogLevel.Info, m);
        public void Debug(string m) => LogInternal(LogLevel.Debug, m);
        public void Warning(string m) => LogInternal(LogLevel.Warning, m);
        public void Trace(string m) => LogInternal(LogLevel.Trace, m);
        public void ObjectInfo(string message, object obj) => LogInternal(LogLevel.Info, message, payload: obj);
        public void Error(string m, Exception? e = null) => LogInternal(LogLevel.Error, m, e);
        public void Fatal(string m, Exception? e = null) => LogInternal(LogLevel.Fatal, m, e);
    }
}
