
namespace UnityCommander.Services
{
    using System;
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
            this.pluginService = loaderService;
            var assembly = Assembly.Load("UnityCommander.Core");
            var command = new IOCommands();
            var arg = new Type[] { command.GetType() };
            var constructor = assembly.GetTypes().Single(t => t.FullName == "UnityCommander.Core.GlobalCommandProvider").GetConstructor(arg);
            // ReSharper disable once CoVariantArrayConversion
            this.globalCommandProvider = constructor?.Invoke(arg);
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
                    foreach (var command in pluginContext.GetCommands())
                    {
                        manager.CreateCommand(command);
                    }

                    foreach (var command in pluginContext.GetPluginCommands())
                    {
                        manager.CreateCommand(command);
                    }
                }

                this.globalCommandManager = commandProvider.GetCommandManager();
            }
        }
    }
}
