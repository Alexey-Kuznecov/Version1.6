
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

        public void InitialCommands() 
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

        public IGlobalCommandManager GetCommandManager<T>() => this.globalCommandManager;
        
        [Obsolete]
        public void SetCommand(UGlobalCommand uGlobal)
        {
            using var xParam = new MultiCommandParameter(uGlobal.ControlItem);

            if (uGlobal.ControlItem is MenuItem menuItem)
            {
                menuItem.Command = uGlobal.Command;
                var header = menuItem.Header;
                    xParam.AddParam((string)header, menuItem, uGlobal.XParamViewModel);
                xParam.ParamFinal(menuItem);
            }
        }
    }
}
