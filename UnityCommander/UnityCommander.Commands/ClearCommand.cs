using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityCommander.CLI.Commands;
using UnityCommander.CLI.Core;
using UnityCommander.CLI.Integration;

namespace UnityCommander.Commands
{
    [ConsoleCommand("clear", "Очищает консоль.", "cls")]
    public class ClearCommand : IConsoleCommand
    {
        public string Name => "clear";
        public IEnumerable<string> Aliases => [ "cls" ];
        public string Description => "Очищает консоль.";

        public Task ExecuteAsync(IConsoleCommandContext context, CancellationToken cancellationToken = default)
        {
            context.Output.Clear(); // Explicitly use System.Console to avoid ambiguity
            return Task.CompletedTask;
        }

        public Task FinalizeAsync()
        {
            return Task.CompletedTask;
        }
    }
}
