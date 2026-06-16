
using Microsoft.Extensions.DependencyInjection;
using System;
using UnityCommander.Abstractions.Overrides;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.Common.Overrides
{
    public sealed class ServiceOverrideResolver
    {
        private readonly IServiceOverrideRegistry _overrideRegistry;

        private readonly IPluginProvider _pluginProvider;
        
        private IServiceProvider _host;

        public ServiceOverrideResolver(
            IServiceProvider serviceProvider, 
            IPluginProvider plugins, 
            IServiceOverrideRegistry registry)
        {
            _overrideRegistry = registry;
            _pluginProvider = plugins;
            _host = serviceProvider;
        }

        public T Resolve<T>()
        {
            var type = typeof(T);

            if (_overrideRegistry.TryGet(type, out var entry))
            {
                var container = _pluginProvider.GetContainer(entry.OwnerId);

                return (T)ActivatorUtilities.CreateInstance(
                    container.Services,
                    entry.ImplementationType);
            }

            return _host.GetRequiredService<T>();
        }
    }
}
