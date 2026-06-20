
using Prism.Ioc;
using Prism.Modularity;
using UnityCommander.Abstractions.Resources;
using UnityCommander.Common.Plugins;
using UnityCommander.Common.Styling;
using UnityCommander.Services.Interfaces;
using UnityCommander.Services.Interfaces.Plugins;

namespace UnityCommander
{
    internal class EarlyLoadModule : IModule
    {
        private IPluginProvider _provider;
        private IPluginActivator _activator;
        private IIconSourceRegistry _iconSource;
        private IPluginInfoProvider _providerInfo;

        public void OnInitialized(IContainerProvider provider)
        {
            _providerInfo = provider.Resolve<IPluginInfoProvider>();
            _provider = provider.Resolve<IPluginProvider>();
            _activator = provider.Resolve<IPluginActivator>();
            _iconSource = provider.Resolve<IIconSourceRegistry>();

            _providerInfo.LoadMetadata();

            _activator.Activate("icon-maker-1.0");
            _activator.Activate("multi-column-1.0");
            _activator.Activate("sidebar-ex-1.0");
            _activator.Activate("plugin-test-1.0");
            _activator.Activate("advance-copy-1.0");

            _iconSource.Register(new MaterialIconSource());
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            //throw new NotImplementedException();
        }

        internal void LoadMetadata()
        {
            _providerInfo.LoadMetadata();
        }

        internal void LoadStartupPlugins()
        {
            _provider.LoadAll();
        }
    }
}
