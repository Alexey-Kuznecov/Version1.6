
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

    using UnityCommander.Common.Commands;
    using UnityCommander.Common.Plugins;
    using UnityCommander.Integration.Columns;
    using UnityCommander.Integration.Commands;
    using UnityCommander.Integration.Contracts;
    using UnityCommander.Integration.Dialog;
    using UnityCommander.Integration.Factories;
    using UnityCommander.Integration.Options;
    using UnityCommander.Services.Interfaces;

    /// <summary>
    /// The plugin loader.
    /// </summary>
    public class PluginLoader : IPluginLoader, IPluginServicesRegister
    {
        /// <summary>
        /// The plugin registered.
        /// </summary>
        private readonly IReadOnlyList<IEnumerable<IPluginService>> pluginsRegistered = new List<IEnumerable<IPluginService>>();

        /// <summary>
        /// The plugin meta.
        /// </summary>
        private readonly AssociatedTypes associatedTypes = new ();

        #region Loaded contracts

        /// <summary>
        /// The dialog service.
        /// </summary>
        private IEnumerable<IDialogService> dialogService;

        /// <summary>
        /// The plugin meta.
        /// </summary>
        private IEnumerable<IPluginDescriptor> pluginMeta;

        /// <summary>
        /// The plugin meta.
        /// </summary>
        private IEnumerable<IPluginSettings> pluginSettings;

        /// <summary>
        /// The option builders
        /// </summary>
        private IEnumerable<BaseCommand> commandsBuilder = new List<BaseCommand>();

        /// <summary>
        /// The plugin commands builder.
        /// </summary>
        private IEnumerable<ICommandBase> pluginCommandsBuilder = new List<ICommandBase>();

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
        /// The get global commands.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public IEnumerable<ICommandBase> GetPluginCommands() => this.pluginCommandsBuilder;

        /// <summary>
        /// The get option builders.
        /// </summary>
        /// <returns>
        ///  The interface option builders.
        /// </returns>
        public IEnumerable<BaseCommand> GetCommands() => this.commandsBuilder;

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

            var typesRegister = new AssociatedTypesRegister(this.associatedTypes);

            foreach (var type in assembly.GetTypes())
            {
                if (typeof(IPluginFactory).IsAssignableFrom(type) && !type.IsAbstract)
                {
                    var plugin = Activator.CreateInstance(type) as IPluginFactory;
                    plugin?.Configure(services);
                    plugin?.SetAssociatedTypes(typesRegister);


                    if (typeof(ICommandFactory).IsAssignableFrom(type) && !type.IsAbstract)
                    {
                        var commandBuilder = new CommandBuilder();
                        var command = (ICommandFactory)plugin;
                        command?.CommandFactory(commandBuilder);
                        this.commandsBuilder = commandBuilder.GetCommands();
                        this.pluginCommandsBuilder = commandBuilder.GetPluginCommands();
                    }
                }
            }

            var serviceProvider = services.BuildServiceProvider();
            this.dialogService = serviceProvider.GetServices<IDialogService>();
            this.pluginMeta = serviceProvider.GetServices<IPluginDescriptor>();

            this.Register<IDialogService, IPluginService>(this.dialogService);
            this.Register<IPluginDescriptor, IPluginService>(this.pluginMeta);
            this.Register<IPluginSettings, IPluginService>(serviceProvider.GetServices<IPluginSettings>());
            this.Register<IColumnBuilder, IPluginService>(serviceProvider.GetServices<IColumnBuilder>());
            this.Register<IOptionBuilder, IPluginService>(serviceProvider.GetServices<IOptionBuilder>());
        }

        /// <summary>
        /// Получает коллекцию загружаемых служб, которые были найдены в подключаемых модулях.
        /// </summary>
        /// <typeparam name="T">
        /// Тип службы которую нужна получить.
        /// </typeparam>
        /// <returns>
        /// Возвращает коллекцию загружаемых служб.
        /// </returns>
        public IEnumerable<T> GetServices<T>() where T : IPluginService
        {
            return this.pluginsRegistered.Where(enumerable => enumerable is IEnumerable<T>)
                .SelectMany(registered => registered)
                .Cast<T>();
        }
        
        /// <summary>
        /// Регистрирует службы подключаемых модулей для управления настройками,
        /// получения описания и внедрения реализации расширяющий функционал программы. 
        /// </summary>
        /// <param name="instance">
        /// Экземпляр класса подключаемого модуля который реализует контракт, например <see cref="IDialogService"/>.
        /// </param>
        /// <typeparam name="TI">
        /// Тип службы наследует общий для всех служб <see cref="IPluginService"/> контракт.
        /// </typeparam>
        /// <typeparam name="TS">
        /// Общий контракт служб, на данный момент нужен лишь для того чтобы регистрировать службы в одной категории.
        /// А так же избежать ошибок связанных приведением к типу.
        /// </typeparam>
        public void Register<TI, TS>(IEnumerable<TS> instance)
            where TI : TS
        {
            if (instance is IEnumerable<IPluginService> registered)
            {
                ((List<IEnumerable<IPluginService>>)this.pluginsRegistered).Add(registered);
            }
        }

        /// <summary>
        /// The get associated types.
        /// </summary>
        /// <returns>
        /// The <see cref="AssociatedTypes"/>.
        /// </returns>
        public AssociatedTypes GetAssociatedTypes() => this.associatedTypes;

        /// <summary>
        /// The unload plugin.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool UnloadPlugin()
        {
            this.alc.Unload();
            this.pluginMeta = null;
            this.dialogService = null;
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
