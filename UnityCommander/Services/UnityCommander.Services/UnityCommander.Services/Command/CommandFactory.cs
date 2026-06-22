
using CommandSystem.Abstractions;
using CommandSystem.Gui;
using UnityCommander.Core.Commands;
using UnityCommander.Services.Interfaces.Plugins;
using ICommand = System.Windows.Input.ICommand;

namespace UnityCommander.Services
{
    public class CommandFactory
    {
        private IPluginCommandProvider _pluginCommand;
        private CommandRegistryService _commandRegistry;

        public CommandFactory(
            IPluginCommandProvider pluginCommand,
            CommandRegistryService commandRegistry)
        {
            _pluginCommand = pluginCommand;
            _commandRegistry = commandRegistry;
        }

        public ICommand Create(string commandId)
        {
            if (_commandRegistry.Get(commandId) != null)
            {
                var asyncCommand = _commandRegistry.Get(commandId).Command as IAsyncCommand;

                return new AsyncCommandAdapter(asyncCommand);
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
