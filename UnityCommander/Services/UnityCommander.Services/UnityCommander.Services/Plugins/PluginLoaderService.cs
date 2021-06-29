
namespace UnityCommander.Services.Plugins
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using Integration.Contracts;
    using Integration.Dialog;
    using Interfaces;

    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;

    /// <summary>
    /// The plugin provider service.
    /// </summary>
    public class PluginLoaderService : IPluginLoaderService
    {
        private static readonly List<IPluginLoader> pluginLoaders = new ();

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginLoaderService"/> class.
        /// </summary>
        public PluginLoaderService()
        {
            string mainPath = GetMainPath() + "\\UnityCommander\\bin\\Debug\\netcoreapp3.1\\";
            var pluginsDir = Path.Combine(mainPath, "plugins");
            foreach (var dir in Directory.GetDirectories(pluginsDir))
            {
                var dirName = Path.GetFileName(dir);
                var pluginDll = Path.Combine(dir + "\\netcoreapp3.1\\", dirName + ".dll");

                if (File.Exists(pluginDll))
                {
                    var pluginLoader = new WeakPluginLoader();
                    pluginLoader.ExecuteAndUnload(pluginDll);
                    Console.WriteLine($"{dirName} is loaded.");
                    pluginLoaders.Add(pluginLoader);
                }
            }
        }

        private static string GetMainPath()
        {
            return Path.GetFullPath(Path.Combine(
                Path.GetDirectoryName(
                    Path.GetDirectoryName(
                        Path.GetDirectoryName(
                            Path.GetDirectoryName(
                                Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory))))) ?? string.Empty));
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

    #endregion

        public void UnloadInterface(AssemblyName assemblyName)
        {
            ((List<IPluginConfigure>)ImportPluginSettings).RemoveAll(p 
                => p.GetType().Assembly.GetName().FullName == assemblyName.FullName);
            ((List<IPluginImplement>)ImportPluginImplements).RemoveAll(p
                => p.GetType().Assembly.GetName().FullName == assemblyName.FullName);
            ((List<IDialogService>)ImportDialogService).RemoveAll(p
                => p.GetType().Assembly.GetName().FullName == assemblyName.FullName);
            ((List<IPluginDescriptor>)ImportPluginMeta).RemoveAll(p
                => p.GetType().Assembly.GetName().FullName == assemblyName.FullName);
        }

        public List<IPluginLoader> GetPluginLoaders()
        {
            return pluginLoaders;
        }

        public bool UnloadPlugins()
        {
            bool isLoaded = default(bool);

            foreach (var plugin in pluginLoaders)
            {
                if (plugin.UnloadPlugin())
                {
                    pluginLoaders.Remove(plugin);
                    isLoaded = true;
                    break;
                }
            }
            
            return isLoaded;
        }

        /// <summary>
        /// Gets interfaces to configure plugins.
        /// </summary>
        /// <returns>
        /// The list interfaces to configure plugins.
        /// </returns>
        public IPluginManager GetPluginManager()
        {
            foreach (var plugin in this.ImportPluginMeta)
            {
                if (plugin is not null)
                {
                    Assembly.GetAssembly(plugin.GetType());
                }
            }

            return null;
        }

        /// <summary>
        /// Scans for a plugins folder in its base directory and attempts to load any plugins it finds.
        /// </summary>
        /// <returns> List of loaded plugins. </returns>
        //private List<PluginLoader> GetPluginLoaders()
        //{
        //    var loaders = new List<PluginLoader>();
        //    // Create plugin loaders
        //    var pluginsDir = Path.Combine(AppContext.BaseDirectory, "plugins");
        //    foreach (var dir in Directory.GetDirectories(pluginsDir))
        //    {
        //        var dirName = Path.GetFileName(dir);
        //        var pluginDll = Path.Combine(dir + "\\netcoreapp3.1\\", dirName + ".dll");

        //        //var alcRecords = new ALCRecords
        //        //{
        //        //    Alc = new AssemblyLoadContext(dirName, true),
        //        //    AssemblyName = AssemblyName.GetAssemblyName(pluginDll)
        //        //};

        //        //PluginManager.ALCRecords.Add(alcRecords);

        //        if (File.Exists(pluginDll))
        //        {
        //            var loader = PluginLoader.CreateFromAssemblyFile(
        //                pluginDll,
        //                sharedTypes: new[] { typeof(IPluginFactory), typeof(IServiceCollection) },
        //                (config) =>
        //                {
        //                    // config.IsLazyLoaded = true;
        //                    //config.DefaultContext = alcRecords.Alc;
        //                });
        //            loaders.Add(loader);
        //        }
        //    }

        //    return loaders;
        //}

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
    }
}
