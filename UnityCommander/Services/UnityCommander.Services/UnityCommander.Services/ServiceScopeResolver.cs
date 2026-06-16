
using System;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.Services
{
    public class ServiceScopeResolver : IServiceScopeResolver
    {
        private readonly IServiceProvider _host;
        private readonly IPluginProvider _plugins;

        public ServiceScopeResolver(IServiceProvider host, IPluginProvider plugins)
        {
            _host = host;
            _plugins = plugins;
        }

        public IServiceProvider Resolve(string? ownerId)
        {
            if (string.IsNullOrWhiteSpace(ownerId))
                return _host;

            var container = _plugins.GetContainer(ownerId);

            return container.Services ?? _host;
        }
    }
}
