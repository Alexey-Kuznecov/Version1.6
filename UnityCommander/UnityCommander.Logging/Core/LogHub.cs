namespace UnityCommander.Logging.Core
{
    public sealed class LogHub
    {
        public event Action<LogEntry>? LogReceived;

        public void Publish(LogEntry entry)
        {
            LogReceived?.Invoke(entry);
        }
    }
}
