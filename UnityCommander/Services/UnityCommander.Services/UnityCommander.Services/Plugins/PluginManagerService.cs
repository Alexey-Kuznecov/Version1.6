
namespace UnityCommander.Services.Plugins
{
#if NETCOREAPP3_1
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Extensions.DependencyInjection;
    using UnityCommander.Integration.Contracts;
    using UnityCommander.Integration.Dialog;
    using UnityCommander.Services.Interfaces;

    using System.Runtime.Loader;
    using Plugin.Core;
#endif

    public class PluginManagerService // : IPluginManagerService, IPluginImplementService
    {
#if NETCOREAPP3_1
        private readonly List<AssemblyLoadContext> LoadContext = new List<AssemblyLoadContext>();

        private readonly List<PluginRecord> PluginRecords = new List<PluginRecord>();
        
        private readonly List<ALCRecords> ALCRecords = new List<ALCRecords>();

        private readonly PluginManager PluginManager = new PluginManager();

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginManagerService"/> class.
        /// </summary>
        public PluginManagerService()
        {
            var service = new ServiceCollection();
            var pluginLoaders = this.LoadPlugin();      
            ConfigureServices(service, pluginLoaders);
            using var serviceProvider = service.BuildServiceProvider();
            this.ImportPluginImplements = serviceProvider.GetServices<IPluginImplement>();
            this.ImportPluginSettings = serviceProvider.GetServices<IPluginConfigure>();
            this.ImportDialogService = serviceProvider.GetServices<IDialogService>();
            this.ImportPluginMeta = serviceProvider.GetServices<IPluginDescriptor>();
        }

        #region Import Properties

        /// <summary>
        /// Gets the imported plugin settings.
        /// </summary>
        public IEnumerable<IPluginConfigure> ImportPluginSettings { get; private set; }

        /// <summary>
        /// Gets  the imported plugin implementations.
        /// </summary>
        public IEnumerable<IPluginImplement> ImportPluginImplements { get; private set; }

        /// <summary>
        /// Gets the imported dialog service.
        /// </summary>
        public IEnumerable<IDialogService> ImportDialogService { get; private set; }

        /// <summary>
        /// Gets the imported the name and description for plugin.
        /// </summary>
        public IEnumerable<IPluginDescriptor> ImportPluginMeta { get; private set; }

        #endregion

        #region Implementations Methods

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

        #endregion

        /// <summary>
        /// Gets interfaces to configure plugins.
        /// </summary>
        /// <returns>
        /// The list <see cref="IPluginConfigure"/> interfaces to configure plugins.
        /// </returns>
        public IEnumerable<IPluginRecord> GetPluginManager()
        {
            foreach (var plugin in this.ImportPluginMeta)
            {
                if (plugin is not null)
                {
                    var assembly = Assembly.GetAssembly(plugin.GetType());

                    var record = this.ALCRecords.Single(r => r.AssemblyName.FullName == assembly?.FullName);
                    
                    PluginRecords.Add(new PluginRecord
                    {
                        Name = plugin.DisplayName,
                        Description = plugin.Description,
                        AssemblyName = record.AssemblyName,
                        Token = record.Token
                    });
                }
            }

            return PluginRecords;
        }

        /// <summary>
        /// Scans for a plugins folder in its base directory and attempts to load any plugins it finds.
        /// </summary>
        /// <returns> List of loaded plugins. </returns>
        public List<PluginLoader> LoadPlugin()
        {
            var loaders = new List<PluginLoader>();
            // Create plugin loaders
            var pluginsDir = Path.Combine(AppContext.BaseDirectory, "plugins");
            foreach (var dir in Directory.GetDirectories(pluginsDir))
            {
                var dirName = Path.GetFileName(dir);
                var pluginDll = Path.Combine(dir + "\\netcoreapp3.1\\", dirName + ".dll");

                var alcRecords = new ALCRecords
                {
                    Alc = new AssemblyLoadContext(dirName, true),
                    AssemblyName = AssemblyName.GetAssemblyName(pluginDll)
                };

                this.ALCRecords.Add(alcRecords);
                this.LoadContext.Add(alcRecords.Alc);

                if (File.Exists(pluginDll))
                {
                    var loader = PluginLoader.CreateFromAssemblyFile(
                        pluginDll,
                        sharedTypes: new[] { typeof(IPluginFactory), typeof(IServiceCollection) },
                        (config) =>
                        {
                            config.IsLazyLoaded = true;
                            config.PreferSharedTypes = true;
                            config.DefaultContext = alcRecords.Alc;
                            
                        });
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
        private void ConfigureServices(ServiceCollection services, List<PluginLoader> loaders)
        {
            // Create an instance of plugin types
            foreach (var loader in loaders)
            {
                Assembly defaultAssembly = loader.LoadDefaultAssembly();
                var types = defaultAssembly.GetTypes().Where(t => typeof(IPluginFactory).IsAssignableFrom(t) && !t.IsAbstract);

                foreach (var pluginType in types)
                {
                    // This assumes the implementation of IPluginFactory has a parameter less constructor
                    var plugin = Activator.CreateInstance(pluginType) as IPluginFactory;

                    plugin?.Configure(services);
                    ResourceManager.GetResourceDictionary(pluginType.Assembly);
                }
            }
        }
#endif
    }
}
