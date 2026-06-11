
using PluginSystem.Abstractions.Plugin;
using System.Collections.Generic;
using UnityCommander.Services.Interfaces.Plugins;

namespace UnityCommander.Services.Plugins
{
    public class PluginInfoProvider : IPluginInfoProvider
    {
        private readonly IPluginCatalog _catalog;

        public PluginInfoProvider(IPluginCatalog catalog)
        {
            _catalog = catalog;
        }

        public IReadOnlyCollection<PluginInfo> Plugins
            => _catalog.GetAll();

        public PluginInfo GetInfo(string pluginId)
            => _catalog.Get(pluginId);

        public void LoadMetadata()
            => _catalog.LoadMetadata();
    }
}
