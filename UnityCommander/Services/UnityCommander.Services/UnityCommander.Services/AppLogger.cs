
using System;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.Services
{
    public class AppLogger : IAppLogger
    {
        public event Action<LogEntry> OnLog;

        private void Write(AppLogLevel level, string message)
        {
            var entry = new LogEntry
            {
                Timestamp = DateTime.Now,
                Message = message,
                Level = level,
                Source = GetCallerName()
            };

            OnLog?.Invoke(entry);
        }

        public void Info(string message) => Write(AppLogLevel.Info, message);
        public void Debug(string message) => Write(AppLogLevel.Debug, message);
        public void Warn(string message) => Write(AppLogLevel.Warning, message);
        public void Error(string message) => Write(AppLogLevel.Error, message);

        private string GetCallerName([System.Runtime.CompilerServices.CallerMemberName] string? name = null)
            => name ?? "Unknown";
    }
}
