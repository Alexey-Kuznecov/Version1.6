using UnityCommander.Logging.Core;

namespace UnityCommander.Logging.Contracts
{
    public interface ILogFilter
    {
        bool Allow(LogEntry logEntry);
    }
}
