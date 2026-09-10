
using System.Threading;
using System.Threading.Tasks;
using UnityCommander.CLI.Core;
using UnityCommander.CLI.Integration;
using UnityCommander.CLI.Lifecicle;
using UnityCommander.Commands.Parsing;

namespace UnityCommander.Commands
{
    [ConsoleCommand("stop", "Остонавливает процесс. Напрмер watch", "s")]
    public class StopProcessCommand : IConsoleCommand
    {
        private readonly CommandProcessManager _processManager;
        private readonly ICommandArgumentParser _parser;

        public string Name => "stop";

        public string Description => "Остонавливает процесс. Напрмер watch.";

        public StopProcessCommand(
            CommandProcessManager processManager,
            ICommandArgumentParser parser)
        {
            _processManager = processManager;
            _parser = parser;
        }

        public Task ExecuteAsync(IConsoleCommandContext context, CancellationToken cancellationToken = default)
        {
            var arguments = _parser.Parse(context.Arguments);

            var processName = arguments.GetAt(0);

            if (processName != null)
            {
                _processManager.StopByName(processName);
                return Task.CompletedTask;
            }

            _processManager.StopAll();

            context.Output.WriteLine($"Процесс ({processName}) успешно завершен!");

            return Task.CompletedTask;
        }

        public Task FinalizeAsync()
        {
            return Task.CompletedTask;
        }
    }
}
