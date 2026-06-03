
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityCommander.CLI.Core;
using UnityCommander.CLI.Integration;
using UnityCommander.Commands.Parsing;
using UnityCommander.Commands.Services;

namespace UnityCommander.Commands
{
    [ConsoleCommand("sysstat", "Выводит статистику производительности системы.", "sst")]
    public class SysStatCommand : IConsoleCommand
    {
        private IConsoleOutput _output;
        public string Name => "sysstat";
        public string Description => "Выводит статистику производительности системы.";
        public IEnumerable<string> Aliases => ["sst"];
        
        private readonly ISysStatService _service;
        private readonly ICommandArgumentParser _parser;

        public SysStatCommand(
            ISysStatService service,
            ICommandArgumentParser parser)
        {
            _service = service;
            _parser = parser;
        }

        public Task ExecuteAsync(
            IConsoleCommandContext context,
            CancellationToken cancellationToken)
        {
            var args =
                _parser.Parse(
                    context.Arguments);

            return _service.RunAsync(
                context.Output,
                args,
                cancellationToken);
        }

        public Task FinalizeAsync()
        {
            return Task.CompletedTask;
        }
    }
}
