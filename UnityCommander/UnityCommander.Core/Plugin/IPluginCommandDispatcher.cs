

using System.Threading.Tasks;

namespace UnityCommander.Core.Plugin
{
    public interface IPluginCommandDispatcher
    {
        Task ExecuteAsync(string commandId);
    }
}
