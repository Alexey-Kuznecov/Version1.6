namespace UnityCommander.Logging.Core
{
    public sealed class LogHub
    {
        private readonly List<LogEntry> _buffer = new();
        public event Action<LogEntry>? LogReceived;
        private const int MaxBufferSize = 500;

        public void Publish(LogEntry entry)
        {
            if (LogReceived == null)
            {
                if (_buffer.Count >= MaxBufferSize)
                    _buffer.RemoveAt(0);
                _buffer.Add(entry); 
            }
            else
                LogReceived?.Invoke(entry);
        }

        public void Subscribe(Action<LogEntry> handler)
        {
            LogReceived += handler;

            foreach (var entry in _buffer)
                handler(entry);
        }
    }
}
