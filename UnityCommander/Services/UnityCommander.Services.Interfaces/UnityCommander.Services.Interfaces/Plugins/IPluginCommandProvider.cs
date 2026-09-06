

using PluginSystem.Abstractions.Plugin;

namespace UnityCommander.Services.Interfaces.Plugins
{
    public interface IPluginCommandProvider
    {
        //public bool TryGet(string commandId, out IPluginCommand command);

        public bool TryGet(string commandId, out (IPluginCommand Command, IPluginContext Context) result);

        //public IPluginCommand Get(string commandId);
    }
}
