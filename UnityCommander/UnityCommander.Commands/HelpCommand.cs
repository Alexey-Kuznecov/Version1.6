
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityCommander.CLI.Commands;
using UnityCommander.CLI.Core;
using UnityCommander.CLI.Integration;
using UnityCommander.CLI.Mode;

namespace UnityCommander.Commands
{
    [ConsoleCommand("help", "Показывает список всех доступных команд с их описаниями.","?", "h")]
    public class HelpCommand : IConsoleCommand
    {
        private ConsoleCommandCatalog _catalog;

        private IServiceProvider _service;

        public string Name => "help";

        public string Description => "Показывает все доступные команды и их описания.";

        public IEnumerable<string> Aliases => ["?", "h"];

        public CommandExecutionMode Mode => CommandExecutionMode.Blocking;

        public HelpCommand(IServiceProvider service, ConsoleCommandCatalog catalog)
        {
            _catalog = catalog;
            _service = service;
        }

        public async Task ExecuteAsync(IConsoleCommandContext context, CancellationToken cancellationToken = default)
        {
            if (_catalog == null)
            {
                context?.Output.WriteLine("Службы не инициализированы.");
                return;
            }
            var commands = _catalog.GetAllCommands()
                .OrderBy(c => c.CommandName)
                .ToList();

            if (commands.Count == 0)
            {
                context?.Output.WriteLine("Нет зарегистрированных команд.");
                return;
            }

            context?.Output.WriteLine("Доступные команды:\n");

            foreach (var command in commands)
            {
                var aliases = (command?.Aliases != null && command.Aliases.Any())
                    ? $" (алиасы: {string.Join(", ", command.Aliases)})"
                    : "";

                context?.Output.WriteLine($"- {command?.CommandName}{aliases}:");
                context?.Output.WriteLine($"  {command?.Description}\n");
            }

            await Task.CompletedTask;
        }

        public Task FinalizeAsync()
        {
            return Task.CompletedTask;
        }
    }
}
