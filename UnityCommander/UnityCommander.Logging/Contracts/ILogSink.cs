using UnityCommander.Logging.Core;

namespace UnityCommander.Logging.Contracts
{ 
    public interface ILogSink
    {
        void Emit(LogEntry entry);
    }
}
