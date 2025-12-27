using UnityCommander.Logging.Configuration;
using UnityCommander.Logging.Contracts;
using UnityCommander.Logging.Core;

namespace UnityCommander.Logging.Sinks
{
    public sealed class FileLogSink : ILogSink, IDisposable
    {
        private readonly string _path;
        private readonly LogChannel _channel;

        public FileLogSink(string path, LogChannel channel)
        {
            File.WriteAllText(path, ""); // очистка файла при создании
            _path = path;
            _channel = channel;
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
