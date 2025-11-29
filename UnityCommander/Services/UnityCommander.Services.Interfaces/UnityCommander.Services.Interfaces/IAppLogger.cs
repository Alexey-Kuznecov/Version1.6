using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityCommander.Services.Interfaces
{
    public interface IAppLogger
    {
        void Info(string message);
        void Debug(string message);
        void Warn(string message);
        void Error(string message);

        event Action<LogEntry> OnLog;
    }

    public class LogEntry
    {
        public DateTime Timestamp { get; set; }
        public string Message { get; set; }
        public AppLogLevel Level { get; set; }
        public string Source { get; set; }
    }

        public enum AppLogLevel
    {
        Debug,
        Info,
        Warning,
        Error
    }
}
