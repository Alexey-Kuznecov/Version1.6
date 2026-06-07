using UnityCommander.Logging.Contracts;
using UnityCommander.Logging.Core;

namespace UnityCommander.Services.Interfaces
{
    public interface IAppLogger
    {
        ILogger Create(string category, LogScope scope);
        ILogger For<T>(LogScope scope = default);
        ILogger ForPlugin(string pluginId);
    }
}
