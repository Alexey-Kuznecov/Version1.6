
using UnityCommander.CLI.Core;
using UnityCommander.CLI.Integration.UnityCommander.CLI.Integration;

namespace UnityCommander.CLI.Integration
{
    public class ConsoleCommandInvoker : IConsoleCommandInvoker
    {
        private readonly IConsoleCommandRegistry _registry;

        public ConsoleCommandInvoker(IConsoleCommandRegistry registry)
        {
            _registry = registry;
        }

        public async Task InvokeAsync(string commandName, IConsoleCommandContext context, CancellationToken cancellationToken = default)
        {
            var command = _registry.Find(commandName);

            if (command == null)
            {
                throw new InvalidOperationException($"Command '{commandName}' not found.");
            }
                
            await command.ExecuteAsync(context, cancellationToken);
        }
    }
}
