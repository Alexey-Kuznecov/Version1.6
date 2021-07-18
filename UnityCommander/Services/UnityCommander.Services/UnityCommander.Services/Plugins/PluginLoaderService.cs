
namespace UnityCommander.Services.Plugins
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using Integration.Columns;
    using Integration.Contracts;
    using Integration.Dialog;

    using Interfaces;

    /// <summary>
    /// The plugin provider service.
    /// </summary>
    public class PluginLoaderService : IPluginLoaderService
    {
        /// <summary>
        /// The plugin loaders.
        /// </summary>
        private static readonly IReadOnlyList<IPluginLoader> PluginLoaders = new List<IPluginLoader>();

        /// <summary>
        /// The plugin load contexts.
        /// </summary>
        private readonly List<IPluginContext> pluginLoadContexts = new ();

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
                    var pluginLoader = new PluginLoader();
                    pluginLoader.ExecuteAndUnload(pluginDll);
                    Console.WriteLine($"{dirName} is loaded.");
                    ((List<IPluginLoader>)PluginLoaders).Add(pluginLoader);
                }
            }

            this.CreatePluginContext();
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
            foreach (var plugin in PluginLoaders)
            {
                foreach (var builder in plugin.GetColumnBuilders())
                {
                    yield return builder;
                }
            }
        }

        public IEnumerable<Column> GetColumn()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Obtain interfaces to implement plugin functionality.
        /// </summary>
        /// <returns>
        /// List of plugin implementations.
        /// </returns>
        public IEnumerable<IPluginImplement> GetPluginImplements()
        {
            foreach (var plugin in PluginLoaders)
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
            foreach (var loader in PluginLoaders)
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
            foreach (var loader in PluginLoaders)
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

        /// <summary>
        /// The get plugin loaders.
        /// </summary>
        /// <returns>
        /// The interface plugin loaders
        /// </returns>
        public List<IPluginLoader> GetPluginLoaders() => (List<IPluginLoader>)PluginLoaders;

        /// <summary>
        /// The get plugin context.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The interface <see cref="IPluginContext"/>.
        /// </returns>
        public IPluginContext GetPluginContext(int index) => this.pluginLoadContexts[index];

        /// <summary>
        /// The get plugin context.
        /// </summary>
        /// <returns>
        /// List of interfaces <see cref="IPluginContext"/>.
        /// </returns>
        public IEnumerable<IPluginContext> GetPluginContext() => this.pluginLoadContexts;

        /// <summary>
        /// The create plugin context.
        /// </summary>
        public void CreatePluginContext()
        {
            foreach (var loader in PluginLoaders)
            {
                var builders = loader.GetColumnBuilders().ToList();
                
                foreach (var nameSpace in this.GetPluginSpaces(builders).Distinct())
                {
                    var pluginContext = new PluginContext();
                    var filterBuilder = builders.Where(b => b.GetType().Namespace == nameSpace);

                    foreach (var builder in filterBuilder)
                    {
                        pluginContext.AddColumn(builder);
                    }

                    this.pluginLoadContexts.Add(pluginContext);
                }
            }
        }

        /// <summary>
        /// The get plugin spaces.
        /// </summary>
        /// <param name="builders">
        /// The builders.
        /// </param>
        /// <returns>
        /// The namespaces.
        /// </returns>
        public HashSet<string> GetPluginSpaces(IEnumerable<IColumnBuilder> builders)
        {
            HashSet<string> namespaces = new ();

            foreach (var builder in builders)
            {
                namespaces.Add(builder.GetType().Namespace);
            }
           
            return namespaces;
        }

        /// <summary>
        /// The unload plugins.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool UnloadPlugins()
        {
            this.ClearHashTable();

            bool isLoaded = default(bool);
            
            Research:
            foreach (var plugin in PluginLoaders)
            {
                if (plugin.UnloadPlugin())
                {
                    isLoaded = true;
                }

                ((List<IPluginLoader>)PluginLoaders).Remove(plugin);
                goto Research;
            }
            
            return isLoaded;
        }

        /// <summary>
        /// The get main path.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string GetMainPath()
            => Path.GetFullPath(Path.Combine(
                Path.GetDirectoryName(
                    Path.GetDirectoryName(
                        Path.GetDirectoryName(
                            Path.GetDirectoryName(
                                Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory))))) ?? string.Empty));

        /// <summary>
        /// The clear hash table.
        /// </summary>
        private void ClearHashTable()
        {
            var typeConverterAssembly = typeof(TypeConverter).Assembly;
            var reflectTypeDescriptionProviderType = typeConverterAssembly.GetType("System.ComponentModel.ReflectTypeDescriptionProvider");

            if (reflectTypeDescriptionProviderType != null)
            {
                var reflectTypeDescriptorProviderTable = reflectTypeDescriptionProviderType.GetField("s_attributeCache", BindingFlags.Static | BindingFlags.NonPublic);
                if (reflectTypeDescriptorProviderTable != null)
                {
                    var attributeCacheTable = (Hashtable)reflectTypeDescriptorProviderTable.GetValue(null);
                    if (attributeCacheTable != null) attributeCacheTable.Clear();
                }
            }
        }
    }
}
