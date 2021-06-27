#define Unity

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using UnityCommander.Integration.Contracts;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.Services.Plugins.NETCORE3_1
{
#if NETCOREAPP3_1
    /// <summary>
    /// The plugin provider service.
    /// </summary>
    public class PluginManagerService : IPluginManagerService
    {
        public IEnumerable<IPluginConfigure> ImportPluginSettings => throw new NotImplementedException();

        public IEnumerable<IPluginDescriptor> ImportPluginMeta => throw new NotImplementedException();

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginManagerService"/> class.
        /// </summary>
        public PluginManagerService()
        {
            var pluginsDir = Path.Combine(AppContext.BaseDirectory, "plugins");
            foreach (var dir in Directory.GetDirectories(pluginsDir))
            {
            }
        }

        public IPluginManager GetPluginManager()
        {
            return new PluginManager(@"UnityCommander\bin\Debug\netcoreapp3.1\plugins\ImagesColumns\netcoreapp3.1\ImagesColumns.dll");
        }
    }
#endif
}
