
using PluginSystem.Runtime;
using System.Collections.Generic;

namespace UnityCommander.Services.Interfaces
{
    /// <summary>
    /// The PluginProvider interface.
    /// </summary>
    public interface IPluginProvider
    {
        PluginContainer GetContainer(string pluginId);

        bool Load(string idOrPath);

        IEnumerable<PluginContainer> LoadAll();

        bool Unload(string pluginId);

        void UnloadAll();
    }
}
