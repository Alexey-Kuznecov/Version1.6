
using PluginSystem.Abstractions.Plugin;
using System.Collections.Generic;
using System.Linq;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.Services
{
    public class PluginProvider : IPluginProvider
    {
        private readonly IPluginManager _manager;

        public PluginProvider(IPluginManager manager)
        {
            _manager = manager;
        }

        public IReadOnlyCollection<PluginInfo> Plugins
            => _manager.GetAllPlugins()
                       .Select(p => p.PluginInfo)
                       .ToArray();

        public IReadOnlyCollection<PluginInfo> LoadedPlugins
            => _manager.GetAllPlugins()
                       .Where(p => p.IsLoaded)
                       .Select(p => p.PluginInfo)
                       .ToArray();

        public bool Load(string idOrPath)
            => _manager.LoadPlugin(idOrPath);

        public IEnumerable<IPluginContainer> LoadAll()
            => _manager.LoadAllPlugins();

        public IEnumerable<IPluginContainer> GetAll()
            => _manager.GetAllPlugins();

        public bool Unload(string pluginId)
            => _manager.UnloadPlugin(pluginId);

        public void UnloadAll()
        {
            foreach (var plugin in _manager.GetAllPlugins())
                _manager.UnloadPlugin(plugin.PluginInfo.DeveloperID);
        }

        public PluginInfo GetInfo(string pluginId)
            => _manager.GetPluginInfo(pluginId);
    }
}
