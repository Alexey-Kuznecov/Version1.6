using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using UnityCommander.Common;

namespace UnityCommander.Core
{
    public static class GlobalCommandExtension
    {
        public static ItemsControl SetParam(this ItemsControl control, string header, string commandName, Action<CommandParametersManager> callback)
        {
            var parametersManager = new CommandParametersManager();
            
            if (control is MenuItem menuItem)
            {
                menuItem.Header = header;
            }
            
            callback(parametersManager);
            ParamBuilder(commandName, control, parametersManager.Params);

            return control;
        }

        public static void ParamBuilder(string commandName, object itemTarget, List<XParamViewModel> vmSource)
        {
            var command = GlobalCommandProvider.FindCommand(commandName);
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
