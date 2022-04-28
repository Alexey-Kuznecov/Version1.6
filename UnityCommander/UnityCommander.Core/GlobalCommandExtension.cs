using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace UnityCommander.Core
{
    using UnityCommander.Common.Commands;

    public static class GlobalCommandExtension
    {
        public static Control SetParam(this Control control, IGlobalCommand command, Action<CommandParametersManager> callback)
        {
            var parametersManager = new CommandParametersManager();
            
            switch (control)
            {
                case MenuItem menuItem:
                    menuItem.Header = command.Name;
                    break;
                case Button button:
                    button.Content = command.Name;
                    break;
            }

            callback(parametersManager);
            // TODO: ParamBuilder(command, control, parametersManager.Params);
            return control;
        }

        public static void ParamBuilder(IGlobalCommand globalCommand, Control controlTarget, List<XParamViewModel> vmSource)
        {
            var command = globalCommand.Command is null ? GlobalCommandProvider.FindCommand(globalCommand.Name) : globalCommand;
            // TODO: Выпилить Delegate из GlobalCommand
            //var paramInfo = command.Delegate.Method.GetParameters();
            using var multiCommandParameter = new MultiCommandParameter(controlTarget);
            var header = default(string);

            switch (controlTarget)
            {
                case MenuItem menuItem:
                    header = (string)menuItem.Header;
                    menuItem.Command = command.Command;
                    break;
                case Button button:
                    header = (string)button.Content;
                    button.Command = command.Command;
                    break;
            }

            //for (var i = 0; i < paramInfo.Length; i++)
            //    multiCommandParameter.AddParam(header, controlTarget, vmSource[i]);
            //multiCommandParameter.ParamFinal(controlTarget);
        }
    }
}
