
using System.Collections;
using System.ComponentModel;

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
    using UnityCommander.Integration.Columns;

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
        public IEnumerable<IColumnBuilder> GetColumnBuilders()
        {
            foreach (var plugin in pluginLoaders)
            {
                foreach (var builder in plugin.GetColumnBuilders())
                {
                    yield return builder;
                }
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
            foreach (var plugin in pluginLoaders)
            {
                foreach (var implement in plugin.GetImplements())
                {
                    yield return implement;
                }
            }
        }

        /// <summary>
        /// Obtain interfaces to implement plugin functionality.
        /// </summary>
        /// <returns>
        /// List of plugin implementations.
        /// </returns>
        public IEnumerable<IDialogService> GetDialogService()
        {
            foreach (var loader in pluginLoaders)
            {
                foreach (var dialog in loader.GetDialogs())
                {
                    yield return dialog;
                }
            }
        }

        /// <summary>
        /// Gets interfaces to configure plugins.
        /// </summary>
        /// <returns>
        /// The list <see cref="IPluginConfigure"/> interfaces to configure plugins.
        /// </returns>
        public IEnumerable<IPluginDescriptor> GetPluginDescriptors()
        {
            foreach (var loader in pluginLoaders)
            {
                foreach (var descriptor in loader.GetDescriptors())
                {
                    yield return descriptor;
                }
            }
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

        public List<IPluginLoader> GetPluginLoaders()
        {
            return pluginLoaders;
        }

        public bool UnloadPlugins()
        {
            this.ClearHashTable();

            bool isLoaded = default(bool);
            
            Research:
            foreach (var plugin in pluginLoaders)
            {
                if (plugin.UnloadPlugin())
                {
                    isLoaded = true;
                }

                pluginLoaders.Remove(plugin);
                goto Research;
            }
            
            return isLoaded;
        }


        void ClearHashTable()
        {
            var typeConverterAssembly = typeof(TypeConverter).Assembly;
            var reflectTypeDescriptionProviderType = typeConverterAssembly.GetType("System.ComponentModel.ReflectTypeDescriptionProvider");

            if (reflectTypeDescriptionProviderType != null)
            {
                var reflectTypeDescriptorProviderTable = reflectTypeDescriptionProviderType.GetField("s_attributeCache", BindingFlags.Static | BindingFlags.NonPublic);
                if (reflectTypeDescriptorProviderTable is not null)
                {
                    var attributeCacheTable = (Hashtable)reflectTypeDescriptorProviderTable.GetValue(null);
                    if (attributeCacheTable != null) attributeCacheTable.Clear();
                }
            }
        }

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
