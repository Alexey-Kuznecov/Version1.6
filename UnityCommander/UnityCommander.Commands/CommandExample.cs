
using System.Threading;
using System.Threading.Tasks;
using UnityCommander.CLI.Core;
using UnityCommander.CLI.Integration;
using UnityCommander.CLI.Mode;
using UnityCommander.Commands.Parsing;

namespace UnityCommander.Commands
{
    [ConsoleCommand("example", "Команда нужна для примера")]
    public class CommandExample : IConsoleCommand
    {
        private ICommandArgumentParser _parser;

        public string Name => "example";
        public string Description => "Команда нужна для примера";

        public CommandExecutionMode Mode 
            => CommandExecutionMode.Immediate;

        public CommandExample(
            ICommandArgumentParser parse)
        {
            _parser = parse;
        }

        public async Task ExecuteAsync(IConsoleCommandContext context, CancellationToken cancellationToken)
        {
            var args = _parser.Parse(context.Arguments);
        }

        public Task FinalizeAsync()
        {
            return Task.CompletedTask;
        }
    }
}
