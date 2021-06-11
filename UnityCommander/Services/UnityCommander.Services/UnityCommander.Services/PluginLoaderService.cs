

namespace UnityCommander.Services
{
    using System;
    using System.Collections.Generic;
#if NET472
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
#endif
    using System.IO;
    using System.Linq;

#if NETCOREAPP3_1
    using Microsoft.Extensions.DependencyInjection;
    using UnityCommander.Integration.Contracts;
    using UnityCommander.Integration.Contracts.Columns;
    using UnityCommander.Plugin.Core;
#else
    using UnityCommander.Integration.Contracts;
    using UnityCommander.Integration.Contracts.Columns;
    using UnityCommander.Plugin48.Core;
#endif
    using UnityCommander.Services.Interfaces;

    /// <summary>
    /// The plugin provider service.
    /// </summary>
    public class PluginLoaderService : IPluginLoaderService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PluginLoaderService"/> class.
        /// </summary>
        public PluginLoaderService()
        {
#if NETCOREAPP3_1
            var services = new ServiceCollection();
            var pluginLoaders = GetPluginLoaders();

            ConfigureServices(services, pluginLoaders);

            using var serviceProvider = services.BuildServiceProvider();
            this.ImportPluginImplements = serviceProvider.GetServices<IPluginImplements>();
            this.ImportPluginSettings = serviceProvider.GetServices<IPluginConfigure>();
            this.ImportColumnServices = serviceProvider.GetServices<IColumnService>();
#endif

#if NET472
            this.SetImport();
#endif
        }

        /// <summary>
        /// Gets or sets the import plugin factories.
        /// </summary>
        public IEnumerable<IPluginConfigure> ImportPluginSettings { get; set; }

        /// <summary>
        /// Gets or sets the import plugin implementations.
        /// </summary>
        public IEnumerable<IPluginImplements> ImportPluginImplements { get; set; }

        /// <summary>
        /// Gets or sets the import plugin implementations.
        /// </summary>
        public IEnumerable<IColumnService> ImportColumnServices { get; set; }

        /// <summary>
        /// Get plugin plugin implementation.
        /// </summary>
        /// <returns>
        /// The <see cref="IPluginImplements"/>.
        /// </returns>
        public IEnumerable<IPluginImplements> GetPluginImplements()
        {
            return this.ImportPluginImplements;
        }

        /// <summary>
        /// The get plugin settings.
        /// </summary>
        /// <returns>
        /// The <see cref="IPluginConfigure"/>.
        /// </returns>
        public IEnumerable<IPluginConfigure> GetPluginSettings()
        {
            return this.ImportPluginSettings;
        }

        /// <summary>
        /// Get column service.
        /// </summary>
        /// <returns>
        /// The <see cref="IPluginConfigure"/>.
        /// </returns>
        public IEnumerable<IColumnService> GetColumnService()
        {
            return this.ImportColumnServices;
        }

        /// <summary>
        /// The get column service.
        /// </summary>
        /// <typeparam name="T">
        /// The contract type
        /// </typeparam>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public IEnumerable<T> GetPluginContract<T>()
        {
            var implement = typeof(T) == typeof(IPluginImplements)
                ? (IEnumerable<T>)this.ImportPluginImplements
                : (IEnumerable<T>)this.ImportPluginSettings;
            return implement;
        }

#if NETCOREAPP3_1
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static List<PluginLoader> GetPluginLoaders()
        {
            var loaders = new List<PluginLoader>();

            // create plugin loaders
            var pluginsDir = Path.Combine(AppContext.BaseDirectory, "plugins");
            foreach (var dir in Directory.GetDirectories(pluginsDir))
            {
                var dirName = Path.GetFileName(dir);
                var pluginDll = Path.Combine(dir + "\\netcoreapp3.1\\", dirName + ".dll");
                if (File.Exists(pluginDll))
                {
                    var loader = PluginLoader.CreateFromAssemblyFile(
                        pluginDll,
                        sharedTypes: new[] { typeof(IPluginFactory), typeof(IServiceCollection) });
                    loaders.Add(loader);
                }
            }

            return loaders;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="loaders"></param>
        private static void ConfigureServices(ServiceCollection services, List<PluginLoader> loaders)
        {
            // Create an instance of plugin types
            foreach (var loader in loaders)
            {
                foreach (var pluginType in loader
                    .LoadDefaultAssembly()
                    .GetTypes()
                    .Where(t => typeof(IPluginFactory).IsAssignableFrom(t) && !t.IsAbstract))
                {
                    // This assumes the implementation of IPluginFactory has a parameter less constructor
                    var plugin = Activator.CreateInstance(pluginType) as IPluginFactory;

                    plugin?.Configure(services);
                }
            }
        }
#endif

#if NET472
        /// <summary>
        /// Configures the import of plug-ins for column expansion.
        /// </summary>
        private void SetImport()
        {
            // An aggregate catalog that combines multiple catalogs
            var catalog = new AggregateCatalog();

            // Add all the parts found in all assemblies in
            // the same directory as the executing program
            foreach (var item in Directory.GetDirectories(Directory.GetCurrentDirectory() + "\\plugins"))
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), item);
                catalog.Catalogs.Add(new DirectoryCatalog(path + @"\net472"));
            }

            // Create the CompositionContainer with the parts in the catalog.
            var container = new CompositionContainer(catalog);

            // Fill the imports of this object
            container.ComposeParts(this);
        }
#endif
    }
}
