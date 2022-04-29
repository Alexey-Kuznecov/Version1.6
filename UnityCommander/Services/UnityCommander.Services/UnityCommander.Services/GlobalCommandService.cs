
namespace UnityCommander.Services
{
    using System.Reflection;

    using UnityCommander.Common.Commands;
    using UnityCommander.Services.Interfaces;
    using UnityCommander.Services.Plugins;

    public class GlobalCommandService : IGlobalCommandService
    {
        private readonly object globalCommandProvider;
        
        private readonly PluginLoaderService pluginService;

        private IGlobalCommandManager globalCommandManager;

        public GlobalCommandService(PluginLoaderService loaderService)
        {
            this.pluginService = loaderService;
            this.globalCommandProvider = Assembly.Load("UnityCommander.Core").CreateInstance("UnityCommander.Core.GlobalCommandProvider");
            this.InitialCommands();
        }

        /// <summary>
        /// The get command manager.
        /// </summary>
        /// <returns>
        /// The <see cref="IGlobalCommandManager"/>.
        /// </returns>
        public IGlobalCommandManager GetCommandManager() => this.globalCommandManager;

        internal void InitialCommands() 
        {
            if (this.globalCommandProvider is IGlobalCommandProvider commandProvider)
            {
                var pluginContexts = pluginService.GetPluginContext();

                var manager = commandProvider.GetCommandManager();

                foreach (var pluginContext in pluginContexts)
                {
                    foreach (var command in pluginContext.GetCommands())
                    {
                        manager.CreateCommand(command);
                    }
                }

                this.globalCommandManager = commandProvider.GetCommandManager();
            }
        }
    }
}
