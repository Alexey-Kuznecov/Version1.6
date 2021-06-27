#define Unity

using System;
using System.Collections.Generic;
using UnityCommander.Integration.Contracts;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.Services.Plugins.NET48
{
#if NET472
    /// <summary>
    /// The plugin provider service.
    /// </summary>
    public class PluginManagerService : IPluginManagerService
    {
        public PluginLoadContext pluginContext { get; private set; }
        public HashSet<WeakReference> pluginContextList { get; private set; } = new HashSet<WeakReference>();

        public IEnumerable<IPluginConfigure> ImportPluginSettings => throw new NotImplementedException();

        public IEnumerable<IPluginDescriptor> ImportPluginMeta => throw new NotImplementedException();

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginManagerService"/> class.
        /// </summary>
        public PluginManagerService()
        {
        }

        public IPluginManager GetPluginManager()
        {
            return new PluginManager("");
        }
    }
#endif
}