
using PluginSystem.Runtime;

namespace UnityCommander.Services.Interfaces.Plugins
{
    public interface IPluginResourceManager
    {
        void Load(PluginContainer container);
        void Unload(PluginContainer container);
    }
}
