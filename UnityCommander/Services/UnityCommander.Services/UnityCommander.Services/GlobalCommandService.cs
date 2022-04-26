
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
        private readonly Assembly assembly;
        
        private readonly object globalCommandProvider;

        private List<GlobalCommand> globalCommands;

        public GlobalCommandService()
        {
            this.assembly = Assembly.Load("UnityCommander.Core");
            this.globalCommandProvider = assembly.CreateInstance("UnityCommander.Core.GlobalCommandProvider");
            this.globalCommands = new List<GlobalCommand>();
        }

        public IGlobalCommandManager GetCommandManager<T>()
        {
            if (this.globalCommandProvider is IGlobalCommandProvider commandProvider)
            {
                var pluginContexts = PluginLoaderService.GetPluginContexts();
                return commandProvider.GetCommandManager<T>();
            }

            return null;
        }

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

        public void SetCommand<T>(string commandName, UGlobalCommand uGlobal)
        {
            var command = this.globalCommands.Single(c => c.Name == commandName);
            var uCommandExecute = (GlobalCommandExecute<T>)command.Command;
            var paramInfo = uCommandExecute.GetCommand().Method.GetParameters();
            using var multiCommandParameter = new MultiCommandParameter(uGlobal.ControlItem);

            if (uGlobal.ControlItem is MenuItem menuItem)
            {
                menuItem.Command = command.Command;
                var header = menuItem.Header;
                for (int i = 0; i < paramInfo.Length; i++)
                    multiCommandParameter.AddParam((string)header, menuItem, uGlobal.XParamModelList[i]);
                multiCommandParameter.ParamFinal(menuItem);
            }       
        }

        public GlobalCommand GetCommand<T>(string commandName)
        {
            var cmd = this.globalCommands.Single(c => c.Name == commandName);
            return cmd;
        }

        public GlobalCommand GetCommand(string commandName)
        {
            var cmd = this.globalCommands.Single(c => c.Name == commandName);
            return cmd;
        }
    }
}
