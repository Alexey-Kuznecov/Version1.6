
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;
using UnityCommander.Common;
using UnityCommander.Integration.Commands;
using UnityCommander.Services.Interfaces;
using UnityCommander.Services.Plugins;

namespace UnityCommander.Services
{
    using UnityCommander.Common.Commands;

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
