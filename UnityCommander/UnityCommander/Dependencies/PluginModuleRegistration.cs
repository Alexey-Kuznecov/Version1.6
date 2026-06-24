
using PluginSystem.Abstractions.Plugin;
using PluginSystem.Abstractions.Settings;
using PluginSystem.Runtime;
using Prism.Ioc;
using System;
using System.IO;
using UnityCommander.Abstractions;
using UnityCommander.Abstractions.Command;
using UnityCommander.Abstractions.Plugin;
using UnityCommander.Abstractions.Plugins;
using UnityCommander.Abstractions.Ribbon;
using UnityCommander.Common.Plugins.UnityCommander.Common.Plugins;
using UnityCommander.Core.Plugin;
using UnityCommander.Core.Registrar;
using UnityCommander.Ribbon.Services;
using UnityCommander.Services.Interfaces;
using UnityCommander.Services.Interfaces.Plugins;
using UnityCommander.Services.Plugins;

namespace UnityCommander.Dependencies
{
    public static class PluginModuleRegistration
    {
        public static void Register(IContainerRegistry registry)
        {
            // PluginSystem
            
            var options = new PluginSystemOptions
            {
                Loader = new PluginLoader(),
                PluginsRootPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins")
            };

            registry.TryRegisterInstance(options);

            registry.RegisterSingleton<IPluginManager, PluginManager>();

            // UnityCommander.Services.Plugins
            registry.RegisterSingleton<IPluginCatalog, PluginCatalog>();
            registry.RegisterSingleton<IPluginInfoProvider, PluginInfoProvider>();
            registry.RegisterSingleton<IPluginProvider, PluginProvider>();
            registry.RegisterSingleton<IPluginActivator, PluginActivator>();
            registry.RegisterSingleton<IRuntimeServices, RuntimeServices>();
            registry.RegisterSingleton<IResourceLoader, BamlResourceLoader>();
            
            registry.RegisterSingleton<PluginHost>();
            registry.RegisterSingleton<ICompositionRegistry, CompositionRegistry>();
            registry.RegisterSingleton<IRibbonBindingRegistry, RibbonBindingRegistry>();
            registry.RegisterSingleton<IPluginCommandRegistry, PluginCommandRegistry>();
            registry.RegisterSingleton<IPluginCommandDispatcher, PluginCommandDispatcher>();
            registry.RegisterSingleton<IPluginCommandProvider, PluginCommandProvider>();
            registry.RegisterSingleton<IRibbonCommandResolver, Core.Commands.RibbonCommandResolver>();
            registry.RegisterSingleton<CompositionEngine>();
        }
    }
}
