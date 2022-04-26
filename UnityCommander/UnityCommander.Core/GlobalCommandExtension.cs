using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using UnityCommander.Common;
using UnityCommander.Integration.Commands;

namespace UnityCommander.Core
{
    public static class GlobalCommandExtension
    {
        public static ItemsControl SetParam(this ItemsControl control, GlobalCommand command, Action<CommandParametersManager> callback)
        {
            var parametersManager = new CommandParametersManager();
            
            if (control is MenuItem menuItem)
            {
                menuItem.Header = command.DisplayName;
            }
            
            callback(parametersManager);
            
            ParamBuilder(command, control, parametersManager.Params);

            return control;
        }

        public static ItemsControl SetParam(this ItemsControl control, string header, ICommand command, Action<CommandParametersManager> callback)
        {
            return null;
        }

        public static void ParamBuilder(GlobalCommand globalCommand, object itemTarget, List<XParamViewModel> vmSource)
        {
            var command = globalCommand.Command is null ? GlobalCommandProvider.FindCommand(globalCommand.CommandName) : globalCommand;
            var paramInfo = command.Delegate.Method.GetParameters();
            using var multiCommandParameter = new MultiCommandParameter(itemTarget);

            if (itemTarget is MenuItem menuItem)
            {
                menuItem.Command = command.Command;
                var header = menuItem.Header;
                for (var i = 0; i < paramInfo.Length; i++)
                    multiCommandParameter.AddParam((string)header, menuItem, vmSource[i]);
                multiCommandParameter.ParamFinal(menuItem);
            }
        }
    }
}
