
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityCommander.CLI.Core;
using UnityCommander.CLI.Integration;
using UnityCommander.CLI.Mode;
using UnityCommander.Commands.Diagnostic;
using UnityCommander.Commands.Parsing;
using UnityCommander.Commands.Rendering;
using UnityCommander.Commands.Services;


namespace UnityCommander.Commands
{
    [ConsoleCommand("inspect", "Выводит список открытых файлов указанного процесса по имени.", "procof")]
    public class InspectCommand : IConsoleCommand
    {
        private IDiagnosticRender _renderer;
        private ICommandArgumentParser _parser;
        private IDiagnosticPipeline _pipeline;
        private WatchService _watchService;

        public string Name => "inspect";

        public string Description => "Выводит список открытых файлов указанного процесса по имени.";

        public IEnumerable<string> Aliases => ["in"];

        public CommandExecutionMode Mode 
            => CommandExecutionMode.Background;

        public InspectCommand(
            ICommandArgumentParser parse,
            IDiagnosticRender render,
            IDiagnosticPipeline pipeline)
        {
            _renderer = render;
            _parser = parse;
            _pipeline = pipeline;
            _watchService = new WatchService(pipeline);
        }

        public async Task ExecuteAsync(IConsoleCommandContext context, CancellationToken cancellationToken)
        {
            var args = _parser.Parse(context.Arguments);

            var interval = args.GetInt("interval");
            var isWatch = args.HasFlag("watch");

            var query = new DiagnosticQuery
            {
                Source = args.GetAt(0),
                Format = args.GetString("format"),
            };

            if (!isWatch)
            {
                var result = _pipeline.Execute(query);
                _renderer.Render(context.Output, result);
                return;
            }

            await _watchService.Run(query, interval, result =>
            {
                context.Output.Clear();
                _renderer.Render(context.Output, result);
            }, cancellationToken);
        }

        public Task FinalizeAsync()
        {
            return Task.CompletedTask;
        }
    }
}
