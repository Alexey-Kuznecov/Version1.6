using CommandSystem.Abstractions;
using CommandSystem.Infrastructure.Lifecycle;
using UnityCommander.CLI.Core;
using UnityCommander.CLI.Integration.UnityCommander.CLI.Integration;
using UnityCommander.CLI.Lifecicle;
using UnityCommander.CLI.Mode;
using UnityCommander.Common.Commands;

namespace UnityCommander.CLI.Integration
{
    public class ConsoleCommandDispatcher
    {
        private readonly IConsoleCommandRegistry _registry;
        private readonly IConsoleCommandInvoker _invoker;
        private readonly ConsoleCommandFactory _factory;
        private readonly CommandProcessManager _processManager;

        public ConsoleCommandDispatcher(
            IConsoleCommandRegistry registry,
            IConsoleCommandInvoker invoker,
            CommandProcessManager processManager,
            ConsoleCommandFactory factory)
        {
            _registry = registry;
            _invoker = invoker;
            _factory = factory;
            _processManager = processManager;
        }

        // Регистрирует команду
        public void RegisterCommand(ConsoleCommandMetadata metadata)
        {
            var command = _factory.Create(metadata);
            _registry.Register(command);
        }

        public void RegisterCommand(IConsoleCommandBase command)
        {
            if (command is not IConsoleCommand console)
                throw new InvalidOperationException("Trying to register non-CLI command in CLI command registry.");

            _registry.Register(console);
        }

        // Выполняет команду
        public async Task ExecuteCommandAsync(
             string commandName,
             IConsoleCommandContext context,
             CancellationToken cancellationToken = default)
        {
            var output = context.Output;

            try
            {
                var command = _registry.Find(commandName);

                if (command == null)
                    throw new InvalidOperationException($"Command '{commandName}' not found.");

                if (command.Mode == CommandExecutionMode.Background)
                {
                    var id = _processManager.Start(commandName, async ct =>
                    {
                        var ctx = new ConsoleCommandContext(
                            context.Services,
                            context.Output,
                            context.Arguments,
                            context.Input);

                        await command.ExecuteAsync(ctx, ct);
                    });

                    output.WriteLine($"Started background process: {id}");
                    return;
                }

                await command.ExecuteAsync(context, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                output.WriteLine("Команда отменена.");
            }
            catch (InvalidOperationException)
            {
                output.WriteError($"Команда '{commandName}' не найдена.");
            }
        }

        public void RegisterDelegateCommand(
            string name,
            string description,
            Func<IConsoleCommandContext, CancellationToken, Task> handler,
            IEnumerable<string>? aliases = null)
        {
            var metadata = new ConsoleCommandMetadata(new CommandMetadata(name, description), handler, aliases?.ToList());
            RegisterCommand(metadata);
        }

        public async Task FinalizeAllCommandsAsync()
        {
            foreach (var cmd in _registry.GetAllCommands())
            {
                if (cmd is IAsyncDisposable asyncDisposable)
                {
                    await asyncDisposable.DisposeAsync();
                }
                else if (cmd is IDisposableCommand disposable)
                {
                    disposable.Dispose();
                }
                else if (cmd is IConsoleCommand cmdWithFinalize)
                {
                    await cmdWithFinalize.FinalizeAsync();
                }
            }
        }

        public void FinalizeAllCommands()
        {
            foreach (var cmd in _registry.GetAllCommands())
            {
                if (cmd is IDisposableCommand disposable)
                {
                    disposable.Dispose();
                }
            }
        }

        public bool TryGetCommand(string cmdName, out object cmd)
        {
            var found = _registry.Find(cmdName);
            if (found != null)
            {
                cmd = found;
                return true;
            }
            cmd = null!;
            return false;
        }

        public IEnumerable<IConsoleCommand> GetAvailableCommands()
              => _registry.GetAllCommands();
    }
}
