using UnityCommander.Logging.Coloring;
using UnityCommander.Logging.Core;

namespace UnityCommander.Logging.Contracts
{
    public interface ILogColorResolver
    {
        LogColor Resolve(LogEntry log);
    }
}
