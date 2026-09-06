using UnityCommander.Logging.Configuration;
using UnityCommander.Logging.Contracts;
using UnityCommander.Logging.Core;

namespace UnityCommander.Logging.Sinks
{
    public sealed class FileLogSink : ILogSink, IDisposable
    {
        private readonly string _path;
        private readonly LogChannel _channel;

        public FileLogSink(string name, LogChannel channel)
        {
            File.WriteAllText(name, ""); // очистка файла при создании
            _path = Path.Combine(Directory.GetCurrentDirectory(), "logs", name);
            _channel = channel;

            var directory = Path.GetDirectoryName(_path);

            if (!string.IsNullOrEmpty(directory))
                Directory.CreateDirectory(directory);

            File.WriteAllText(_path, string.Empty);
        }

        public void Emit(LogEntry entry)
        {
            if (entry.Channel != _channel)
                return;

            var line = Format(entry);

            File.AppendAllText(_path, line + Environment.NewLine);
        }

        private static string Format(LogEntry e)
        {
            return
                $"[{e.Timestamp:yyyy-MM-dd HH:mm:ss.fff}] " +
                $"[{e.Level}] " +
                $"{(string.IsNullOrWhiteSpace(e.Source) ? "" : $"[{e.Source}] ")}" +
                $"{e.Message}";
        }

        public void Dispose()
        {
            // если нужен flush / close
        }
    }
}
