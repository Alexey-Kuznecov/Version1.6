

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
    using UnityCommander.Integration.Dialog;
    using UnityCommander.Plugin.Core;
#else
    using UnityCommander.Integration.Contracts;
    using UnityCommander.Integration.Dialog;
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
            this.ImportPluginImplements = serviceProvider.GetServices<IPluginImplement>();
            this.ImportPluginSettings = serviceProvider.GetServices<IPluginConfigure>();
            this.ImportDialogService = serviceProvider.GetServices<IDialogService>();
#endif

#if NET472
            this.SetImport();
#endif
        }

        #region Imports Plugins Interfaces

        /// <summary>
        /// Gets the imported plugin settings.
        /// </summary>
        public IEnumerable<IPluginConfigure> ImportPluginSettings { get; private set; }

        /// <summary>
        /// Gets  the imported plugin implementations.
        /// </summary>
        public IEnumerable<IPluginImplement> ImportPluginImplements { get; private set; }

        /// <summary>
        /// Gets the import dialog service.
        /// </summary>
        public IEnumerable<IDialogService> ImportDialogService { get; private set; }


        #endregion

        /// <summary>
        /// Obtain an instance of the class that implements the plugin interface.
        /// </summary>
        /// <typeparam name="T">
        /// Required plugin interface.
        /// </typeparam>
        /// <returns>
        /// List class instances.
        /// </returns>
        public IEnumerable<object> GetPluginInstances<T>()
        {
            foreach (var instance in this.GetPluginContract<T>())
            {
                yield return instance;
            }
        }

        /// <summary>
        /// Obtain interfaces to implement plugin functionality.
        /// </summary>
        /// <returns>
        /// List of plugin implementations.
        /// </returns>
        public IEnumerable<IPluginImplement> GetPluginImplements()
        {
            return this.ImportPluginImplements;
        }


        /// <summary>
        /// Obtain interfaces to implement plugin functionality.
        /// </summary>
        /// <returns>
        /// List of plugin implementations.
        /// </returns>
        public IEnumerable<IDialogService> GetDialogService()
        {
            return this.ImportDialogService;
        }

        /// <summary>
        /// Gets interfaces to configure plugins.
        /// </summary>
        /// <returns>
        /// The list <see cref="IPluginConfigure"/> interfaces to configure plugins.
        /// </returns>
        public IEnumerable<IPluginConfigure> GetPluginSettings()
        {
            return this.ImportPluginSettings;
        }

        /// <summary>
        /// Gets a list of the specified plugin interfaces.
        /// </summary>
        /// <typeparam name="T">
        /// Specify the interface <see cref="IPluginImplement"/> or <see cref="IPluginConfigure"/>,
        /// otherwise an exception will be thrown.  
        /// </typeparam>
        /// <returns>
        /// Returns a plugin implementation or plugin settings, otherwise an exception.
        /// </returns>
        public IEnumerable<T> GetPluginContract<T>()
        {
            var implement = typeof(T) == typeof(IPluginImplement)
                ? (IEnumerable<T>)this.ImportPluginImplements
                : (IEnumerable<T>)this.ImportPluginSettings;

            if (implement == null)
            {
                throw new Exception("The specified interface is not a plugin interface.");
            }

            return implement;
        }

#if NETCOREAPP3_1

        /// <summary>
        /// Scans for a plugins folder in its base directory and attempts to load any plugins it finds.
        /// </summary>
        /// <returns> List of loaded plugins. </returns>
        private static List<PluginLoader> GetPluginLoaders()
        {
            var loaders = new List<PluginLoader>();

            // Create plugin loaders
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
        /// Configure plugins in a dependency injection collection.
        /// </summary>
        /// <param name="services"> Service collection. </param>
        /// <param name="loaders"> List of plugins loaded. </param>
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
        /// Configures the import of plugins.
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
