
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityCommander.CLI.Core;
using UnityCommander.CLI.Integration;
using UnityCommander.CLI.Mode;
using UnityCommander.Commands.Parsing;
using UnityCommander.Commands.Services;

namespace UnityCommander.Commands
{
    [ConsoleCommand("console.test-output", "Генерирует тестовый поток сообщений")]
    public sealed class ConsoleTestCommand : IConsoleCommand
    {
        private readonly WatchService _watchService;

        private ICommandArgumentParser _parser;

        public string Name => "console.test-output";

        public CommandExecutionMode Mode
         => CommandExecutionMode.Background;

        public string Description =>
            "Генерирует тестовый поток сообщений";

        public ConsoleTestCommand(ICommandArgumentParser parse)
        {
            _watchService = new WatchService();
            _parser = parse;
        }

        public async Task ExecuteAsync(
            IConsoleCommandContext context,
            CancellationToken cancellationToken)
        {
            var output = context.Output;
            var args = _parser.Parse(context.Arguments);
            var interval = args.GetInt("interval", 100);

            await _watchService.Run(
                interval,
                () =>
                {
                    output.WriteLine(
                        $"[Test] Message generated at {DateTime.Now:HH:mm:ss.fff}");
                },
                cancellationToken);
        }

        public Task FinalizeAsync()
        {
            return Task.CompletedTask;
        }
    }
}
