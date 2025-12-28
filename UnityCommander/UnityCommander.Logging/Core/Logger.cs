using UnityCommander.Logging.Abstractions.Helper;
using UnityCommander.Logging.Configuration;
using UnityCommander.Logging.Contracts;
using System.Collections;

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

        private void LogInternal<T>(
            LogLevel level,
            string message,
            T payload,
            Action<T>? action = null)
        {
            {
                var obj = payload != null ? ObjectLogFormatter.Format(payload) : null;

                var entry = new LogEntry
                {
                    Message = message,
                    Category = _category,
                    Scope = _scope
                };

                _core.Process(entry);
                action?.Invoke(payload);
            } 
        }

        public void Debug(string message, Func<bool>? condition = null) 
            => LogInternal(LogLevel.Debug, message, condition: condition);
        public void Info(string m) => LogInternal(LogLevel.Info, m);
        public void Debug(string m) => LogInternal(LogLevel.Debug, m);
        public void Warning(string m) => LogInternal(LogLevel.Warning, m);
        public void Trace(string m) => LogInternal(LogLevel.Trace, m);
        public void ObjectInfo(string message, object obj, Func<bool>? condition = null) 
            => LogInternal(
                LogLevel.Debug, 
                message, 
                payload: obj,
                condition: condition);
        public void CollectionInfo(string message, IEnumerable collection, Func<bool>? condition = null)
             => LogInternal(
                 LogLevel.Debug, 
                 message, 
                 payload: collection, 
                 condition: condition);
        
        public void ObjectInfo<T>(
            string title,
            T obj,
            Action<T> unpack)
        {
            // защита от говна
            if (unpack == null)
                throw new ArgumentNullException(nameof(unpack));

            if (obj == null)
            {
                Info($"{title}: <null>");
                return;
            }

            LogInternal(
                LogLevel.Debug,
                title,
                payload: obj,
                action: unpack);
            // 🔽 РАЗДЕЛИТЕЛЬ
            LogInternal(
                LogLevel.Debug,
                "--------------------",
                payload: null);
        }

        public void CollectionInfo<T>(
            string title,
            IEnumerable<T> collection,
            Action<T> unpack)
        {
            if (unpack == null)
                throw new ArgumentNullException(nameof(unpack));

            if (collection == null)
            {
                Info($"{title}: <null>");
                return;
            }

            LogInternal(
                LogLevel.Debug,
                title,
                payload: collection,
                action: col =>
                {
                    int index = 0;

                    foreach (var item in col)
                    {
                        if (index > 0)
                        {
                            // 🔽 РАЗДЕЛИТЕЛЬ
                            LogInternal(
                                LogLevel.Debug,
                                "--------------------",
                                payload: null);
                        }

                        LogInternal(
                            LogLevel.Debug,
                            $"[{index}]",
                            payload: item,
                            action: unpack);

                        index++;
                    }
                });
        }

        public void Error(string m, Exception? e = null) => LogInternal(LogLevel.Error, m, e);
        public void Fatal(string m, Exception? e = null) => LogInternal(LogLevel.Fatal, m, e);
    }
}
