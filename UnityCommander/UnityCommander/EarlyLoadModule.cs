
using Prism.Ioc;
using Prism.Modularity;
using System.Windows.Threading;
using UnityCommander.Abstractions.Icons;
using UnityCommander.Abstractions.Keyboard;
using UnityCommander.CLI.Core;
using UnityCommander.CLI.Integration;
using UnityCommander.Common.Styling;
using UnityCommander.Core.Bootstrap;
using UnityCommander.Logging;
using UnityCommander.Logging.Infrastructure;
using UnityCommander.Rendering.Icons;
using UnityCommander.Services;
using UnityCommander.Services.Interfaces;
using UnityCommander.Services.Interfaces.Plugins;
using UnityCommander.WPF.Behaviors;

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
            var loggerCreator = provider.Resolve<LoggerCreator>();

            Log.Initialize(loggerCreator);

            _providerInfo = provider.Resolve<IPluginInfoProvider>();
            _provider = provider.Resolve<IPluginProvider>();
            _activator = provider.Resolve<IPluginActivator>();
            _iconSource = provider.Resolve<IIconSourceRegistry>();
            var iconRender = provider.Resolve<IIconRenderService>();
            var iconColor = provider.Resolve<IIconColorResolver>();
            var context = provider.Resolve<IShortcutContextService>();
           
            IconHub.Initialize(iconRender, iconColor);
            KeyboardBinding.Initialize(context);

            _providerInfo.LoadMetadata();

            _activator.Activate("icon-maker-1.0");
            _activator.Activate("multi-column-1.0");
            //_activator.Activate("sidebar-ex-1.0");
            //_activator.Activate("plugin-test-1.0");
            //_activator.Activate("advance-copy-1.0");

            _iconSource.Register(new MaterialIconSource());
            _iconSource.Register(new FileIconSource("G:\\material.iconpack"));
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
