
using PluginSystem.Abstractions.Plugin;
using PluginSystem.Runtime;
using System.Collections.Generic;

namespace UnityCommander.Common.Plugins
{
    public class PluginProvider : IPluginProvider
    {
        private readonly IPluginManager _manager;

        public PluginProvider(IPluginManager manager)
        {
            _manager = manager;
        }

        public PluginContainer GetContainer(string pluginId)
            => _manager.GetContainerById(pluginId);
        
        public bool Load(string idOrPath)
            => _manager.LoadPlugin(idOrPath);

        public IEnumerable<PluginContainer> LoadAll()
            => _manager.LoadAllPlugins();

        public bool Unload(string pluginId)
            => _manager.UnloadPlugin(pluginId);

        public void UnloadAll()
        {
            foreach (var plugin in _manager.GetAllPlugins())
                _manager.UnloadPlugin(plugin.PluginInfo.DeveloperID);
        }
    }
}
