
using PluginSystem.Abstractions.Plugin;
using System;

namespace UnityCommander.Core.Plugin
{
    public sealed class PluginInstance : IDisposable
    {
        public string PluginId { get; }
        
        public IServiceProvider Services { get; }
        
        public IPluginContext Context { get; }

        public PluginInstance(
            string pluginId, 
            IServiceProvider services, 
            IPluginContext pluginContext)
        {
            PluginId = pluginId;
            Services = services;
            Context = pluginContext;
        }

        public void Dispose()
        {
            if (Services is IDisposable d)
                d.Dispose();
        }
    }
}
