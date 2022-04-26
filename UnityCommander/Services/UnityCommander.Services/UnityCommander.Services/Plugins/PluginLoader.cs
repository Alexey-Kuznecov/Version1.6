
using UnityCommander.Integration.Commands;

namespace UnityCommander.Services.Plugins
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Windows;

    using Microsoft.Extensions.DependencyInjection;

    using UnityCommander.Integration.Columns;
    using UnityCommander.Integration.Contracts;
    using UnityCommander.Integration.Dialog;
    using UnityCommander.Integration.Options;
    using UnityCommander.Services.Interfaces;

    /// <summary>
    /// The plugin loader.
    /// </summary>
    public class PluginLoader : IPluginLoader
    {
        #region Loaded contracts

        /// <summary>
        /// The plugin settings.
        /// </summary>
        private IEnumerable<IPluginConfigure> pluginSettings;

        /// <summary>
        /// The dialog service.
        /// </summary>
        private IEnumerable<IDialogService> dialogService;

        /// <summary>
        /// The plugin meta.
        /// </summary>
        private IEnumerable<IPluginDescriptor> pluginMeta;

        /// <summary>
        /// The column builders.
        /// </summary>
        private IEnumerable<IColumnBuilder> columnBuilders;

        /// <summary>
        /// The option builders
        /// </summary>
        private IEnumerable<IOptionBuilder> optionBuilders;


        /// <summary>
        /// The option builders
        /// </summary>
        private IEnumerable<CommandBase> commandsBuilder = new List<CommandBase>();

        #endregion

        /// <summary>
        /// The alc.
        /// </summary>
        private HostPluginLoadContext alc;

        /// <summary>
        /// The weak reference.
        /// </summary>
        private WeakReference weakReference;

        /// <summary>
        /// The plugin resources.
        /// </summary>
        private HashSet<ResourceDictionary> pluginResources = new ();

        #region Getter methods

        /// <summary>
        /// The get configurations.
        /// </summary>
        /// <returns>
        /// The interface plugin configure.
        /// </returns>
        public IEnumerable<IPluginConfigure> GetConfigurations() => this.pluginSettings;

        /// <summary>
        /// The get descriptors.
        /// </summary>
        /// <returns>
        /// The interface plugin descriptor.
        /// </returns>
        public IEnumerable<IPluginDescriptor> GetDescriptors() => this.pluginMeta;

        /// <summary>
        /// The get dialogs.
        /// </summary>
        /// <returns>
        /// The interface dialog service.
        /// </returns>
        public IEnumerable<IDialogService> GetDialogs() => this.dialogService;

        /// <summary>
        /// The get column builders.
        /// </summary>
        /// <returns>
        /// The interface column builder.
        /// </returns>
        public IEnumerable<IColumnBuilder> GetColumnBuilders() => this.columnBuilders;

        /// <summary>
        /// The get option builders.
        /// </summary>
        /// <returns>
        ///  The interface option builders.
        /// </returns>
        public IEnumerable<IOptionBuilder> GetOptionBuilders() => this.optionBuilders;


        /// <summary>
        /// The get option builders.
        /// </summary>
        /// <returns>
        ///  The interface option builders.
        /// </returns>
        public IEnumerable<CommandBase> GetPluginCommands() => this.commandsBuilder;

        #endregion

        /// <summary>
        /// The execute and unload.
        /// </summary>
        /// <param name="assemblyPath">
        /// The assembly path.
        /// </param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void ExecuteAndUnload(string assemblyPath)
        {
            this.alc = new HostPluginLoadContext(assemblyPath);
            var services = new ServiceCollection();
            this.weakReference = new WeakReference(this.alc);
            Assembly assembly = this.alc.LoadFromAssemblyPath(assemblyPath);
            this.GetPluginResources(assembly);

            foreach (var type in assembly.GetTypes())
            {
                if (typeof(IPluginFactory).IsAssignableFrom(type) && !type.IsAbstract)
                {
                    var plugin = Activator.CreateInstance(type) as IPluginFactory;
                    
                    plugin?.Configure(services);

                    var serviceProvider = services.BuildServiceProvider();
                    this.pluginSettings = serviceProvider.GetServices<IPluginConfigure>();
                    this.pluginMeta = serviceProvider.GetServices<IPluginDescriptor>();
                    this.dialogService = serviceProvider.GetServices<IDialogService>();
                    this.columnBuilders = serviceProvider.GetServices<IColumnBuilder>();
                    this.optionBuilders = serviceProvider.GetServices<IOptionBuilder>();
                    
                    if (typeof(ICommandFactory).IsAssignableFrom(type) && !type.IsAbstract)
                    {
                        var commandBuilder = new CommandBuilder();
                        var command = (ICommandFactory)plugin;
                        command?.CommandFactory(commandBuilder);
                        this.commandsBuilder = commandBuilder.GetCommands();
                    }
                }
            }
        }

        /// <summary>
        /// The unload plugin.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool UnloadPlugin()
        {
            this.alc.Unload();
            this.pluginSettings = null;
            this.pluginMeta = null;
            this.dialogService = null;
            this.columnBuilders = null;
            this.optionBuilders = null;
            this.alc = null;

            if (this.pluginResources?.Count > 0 && this.pluginResources != null)
            {
                foreach (var resource in this.pluginResources)
                {
                    var dictionary = Application.Current.Resources.MergedDictionaries;
                    dictionary.Remove(resource);
                }
            }

            this.pluginResources = null;

            for (var i = 0; this.weakReference.IsAlive && (i < 10); i++)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            Debug.WriteLine($"Unload success: {!this.weakReference.IsAlive}");
            return !this.weakReference.IsAlive;
        }

        /// <summary>
        /// The get plugin resources.
        /// </summary>
        /// <param name="assembly">
        /// The assembly.
        /// </param>
        private void GetPluginResources(Assembly assembly)
        {
            this.pluginResources = PluginResourceManager.GetResourceDictionary(assembly);

            if (this.pluginResources?.Count != 0 && this.pluginResources != null)
            {
                var dictionary = Application.Current.Resources.MergedDictionaries;

                foreach (var resource in this.pluginResources)
                {
                    dictionary.Add(resource);
                }
            }
        }
    }
}
