
using PluginSystem.Abstractions.Plugin;
using PluginSystem.Runtime;
using Prism.Ioc;
using UnityCommander.Integration.Plugins;
using UnityCommander.Services;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.Dependencies
{
    public static class PluginModuleRegistration
    {
        public static void Register(IContainerRegistry registry)
        {
            registry.RegisterSingleton<IPluginManager, PluginManager>();
            registry.RegisterSingleton<IPluginProvider, PluginProvider>();
        }
    }
}
