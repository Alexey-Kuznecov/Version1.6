
using System.Windows.Input;
using UnityCommander.Modules.ToolBar.Commands;
using UnityCommander.Ribbon.Services;
using UnityCommander.Services;
using UnityCommander.Services.Interfaces.Plugins;

namespace UnityCommander.Core.Commands
{
    public class RibbonCommandResolver : IRibbonCommandResolver
    {
        private IPluginCommandProvider _pluginCommand;
        private CommandRegistryService _commandRegistry;

        public RibbonCommandResolver(
            IPluginCommandProvider pluginCommand, 
            CommandRegistryService commandRegistry)
        {
            _pluginCommand = pluginCommand;
            _commandRegistry = commandRegistry;
        }

        public ICommand Resolve(string commandId)
        {

            if (_commandRegistry.Get(commandId) != null) 
            {
                return _commandRegistry.Get(commandId).Command as ICommand;
            }

            if (_pluginCommand.TryGet(commandId, out var result))
            {
                var command = result.Command;
                var context = result.Context;

                return new PluginCommand(command, context);
            }

            return null;
        }
    }
}
