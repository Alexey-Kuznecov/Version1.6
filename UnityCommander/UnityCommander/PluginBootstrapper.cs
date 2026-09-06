
using PluginSystem.Abstractions.Plugin;
using UnityCommander.Services.Interfaces.Plugins;

namespace UnityCommander
{
    internal class PluginBootstrapper
    {
        private IPluginManager _manager;
        private IPluginInfoProvider _provider;

        public PluginBootstrapper(IPluginInfoProvider provider)
        {
            _provider = provider;
        }

        internal void LoadMetadata()
        {
            _manager.LoadMetadata();
        }

        internal void LoadStartupPlugins()
        {
            _manager.LoadAllPlugins();
        }
    }
}