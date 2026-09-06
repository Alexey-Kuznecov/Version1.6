
using UnityCommander.Core.Commands;
using UnityCommander.Services.Interfaces.Plugins;

namespace UnityCommander.Services.Command
{
    public class CommandExecuter : ICommandExecuter
    {
        private IPluginCommandProvider _pluginCommand;
        private CommandRegistryService _commandRegistry;
        private CommandExecutionService _execution;

        public CommandExecuter(
            IPluginCommandProvider pluginCommand,
            CommandRegistryService commandRegistry, 
            CommandExecutionService executionService)
        {
            _pluginCommand = pluginCommand;
            _commandRegistry = commandRegistry;
            _execution = executionService;
        }

        public void Execute(string commandId)
        {
            if (_commandRegistry.Get(commandId) != null)
            {
                _execution.ExecuteAsync(commandId);
            }

            if (_pluginCommand.TryGet(commandId, out var result))
            {
                var command = result.Command;
                var context = result.Context;

                command.ExecuteAsync(context);
            }
        }
    }
}
