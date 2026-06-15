
using PluginSystem.Runtime;
using System.Collections.Generic;

namespace UnityCommander.Common.Plugins
{
    public interface IPluginProvider
    {
        PluginContainer GetContainer(string pluginId);

        bool Load(string idOrPath);

        IEnumerable<PluginContainer> LoadAll();

        bool Unload(string pluginId);

        void UnloadAll();
    }
}
