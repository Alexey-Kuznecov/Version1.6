
namespace UnityCommander.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using UnityCommander.Common.Commands;
    using UnityCommander.Integration.Commands;
    using UnityCommander.Services.Interfaces;
    using UnityCommander.Services.Plugins;

    /// <summary>
    /// The global command service.
    /// </summary>
    public class GlobalCommandService : IGlobalCommandService
    {
        /// <summary>
        /// The base commands.
        /// </summary>
        private List<BaseCommand> BaseCommands = new ();

        /// <summary>
        /// The global command provider.
        /// </summary>
        private readonly object globalCommandProvider;

        /// <summary>
        /// The plugin service.
        /// </summary>
        private readonly PluginLoaderService pluginService;

        /// <summary>
        /// The global command manager.
        /// </summary>
        private IGlobalCommandManager globalCommandManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="GlobalCommandService"/> class.
        /// </summary>
        /// <param name="loaderService">
        /// The loader service.
        /// </param>
        public GlobalCommandService(PluginLoaderService loaderService)
        {
            this.BaseCommands.Add(new IOCommands());
            this.pluginService = loaderService;
            var assembly = Assembly.Load("UnityCommander.Core");

            var constructor = assembly.GetTypes().Single(t => t.FullName == "UnityCommander.Core.GlobalCommandProvider").GetConstructor(Type.EmptyTypes);
            // ReSharper disable once CoVariantArrayConversion
            this.globalCommandProvider = constructor?.Invoke(null);
            this.InitialCommands();
        }

        /// <summary>
        /// The get command manager.
        /// </summary>
        /// <returns>
        /// The <see cref="IGlobalCommandManager"/>.
        /// </returns>
        public IGlobalCommandManager GetCommandManager() => this.globalCommandManager;

        /// <summary>
        /// The initial commands.
        /// </summary>
        internal void InitialCommands() 
        {
            if (this.globalCommandProvider is IGlobalCommandProvider commandProvider)
            {
                var pluginContexts = this.pluginService.GetPluginContext();

                var manager = commandProvider.GetCommandManager();

                foreach (var pluginContext in pluginContexts)
                {
                    //foreach (var command in pluginContext.GetCommands())
                    //{
                    //    manager.CreateCommand(command);
                    //}

                    foreach (var command in pluginContext.GetPluginCommands())
                    {
                        manager.CreateCommand(command);
                    }
                }

                foreach (var command in this.BaseCommands)
                {
                    manager.CreateCommand(command);
                }

                this.globalCommandManager = commandProvider.GetCommandManager();
            }
        }
    }
}
