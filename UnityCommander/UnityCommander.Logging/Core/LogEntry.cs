using UnityCommander.Logging.Coloring;
using UnityCommander.Logging.Configuration;

namespace UnityCommander.Logging.Core
{
    public class LogEntry
    {
        public DateTime Timestamp { get; init; }
        public LogLevel Level { get; init; }
        public LogChannel Channel { get; init; }
        public string Message { get; init; } = "";
        public string? Source { get; init; }
        public Exception? Exception { get; init; }
        public string Scope { get; init; } = "";
        public string Category { get; init; } = "";
        public object? Payload { get; init; }

        // 👇 вот тут всё сходится
        public LogColor? Color { get; set; }
        public Func<bool>? Condition { get; init; }
    }
}
