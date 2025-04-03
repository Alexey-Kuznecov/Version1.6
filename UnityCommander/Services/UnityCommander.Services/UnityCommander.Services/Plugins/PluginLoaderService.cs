
namespace UnityCommander.Services.Plugins
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using Integration.Contracts;
    using Integration.Dialog;

    using Interfaces;

    using UnityCommander.Integration.Columns;
    using UnityCommander.Integration.Options;

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
        private readonly List<IPluginContext> pluginContexts = new ();

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginLoaderService"/> class.
        /// </summary>
        public PluginLoaderService()
        {
            string mainPath = GetMainPath();
            string pluginsDir = Path.Combine(mainPath, "plugins");

            if (!Directory.Exists(pluginsDir))
            {
                Console.WriteLine("Plugins directory not found.");
                return;
            }

            LoadPlugins(pluginsDir);

            this.CreatePluginContext();
        }

        private void LoadPlugins(string pluginsDir)
        {
            foreach (var dir in Directory.GetDirectories(pluginsDir))
            {
                string dirName = Path.GetFileName(dir);
                string pluginDll = Path.Combine(dir, "netcoreapp3.1", $"{dirName}.dll");

                if (File.Exists(pluginDll))
                {
                    var pluginLoader = new PluginLoader();
                    pluginLoader.ExecuteAndUnload(pluginDll);
                    Console.WriteLine($"{dirName} is loaded.");
                    ((List<IPluginLoader>)PluginLoaders).Add(pluginLoader);
                }
                else
                {
                    Console.WriteLine($"Plugin DLL not found: {pluginDll}");
                }
            }

            this.CreatePluginContext();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string GetMainPath()
        {
            return AppContext.BaseDirectory; // Универсальный путь, учитывает публикацию и окружение
        }


        #region Implementations Methods

        /// <summary>
        /// Obtain interfaces to implement plugin functionality.
        /// </summary>
        /// <returns>
        /// List of plugin implementations.
        /// </returns>
        public IEnumerable<IDialogService> GetDialogService() 
            => PluginLoaders.SelectMany(loader => loader.GetDialogs());

        /// <summary>
        /// Gets interfaces to configure plugins.
        /// </summary>
        /// <returns>
        /// The list <see cref="IPluginConfigure"/> interfaces to configure plugins.
        /// </returns>
        public IEnumerable<IPluginDescriptor> GetPluginDescriptors() 
            => PluginLoaders.SelectMany(loader => loader.GetDescriptors());

        #endregion

        /// <summary>
        /// The get plugin context.
        /// </summary>
        /// <returns>
        /// List of interfaces <see cref="IPluginContext"/>.
        /// </returns>
        public IEnumerable<IPluginContext> GetPluginContext() => this.pluginContexts;

        /// <summary>
        /// The create plugin context.
        /// </summary>
        public void CreatePluginContext()
        {
            foreach (var loader in PluginLoaders)
            {
                var pluginContext = new PluginContext(loader.GetAssociatedTypes());
                var pluginContextBuilder = new PluginContextBuilder(pluginContext);

                foreach (var columnBuilder in loader.GetServices<IColumnBuilder>())
                {
                    pluginContextBuilder.AddColumn(columnBuilder);
                }

                foreach (var optionBuilder in loader.GetServices<IOptionBuilder>())
                {
                    pluginContextBuilder.AddOption(optionBuilder);
                }

                foreach (var pluginSettings in loader.GetServices<IPluginSettings>())
                {
                    var dd = pluginSettings;
                }

                var pluginCommands = loader.GetPluginCommands().ToList();
                var commands = loader.GetCommands().ToList();

                pluginContextBuilder.AddPluginCommand(pluginCommands);
                pluginContextBuilder.AddCommand(commands);
                this.pluginContexts.Add(pluginContext);
            }
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
