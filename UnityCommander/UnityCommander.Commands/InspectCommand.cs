
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using UnityCommander.CLI.Core;
using UnityCommander.CLI.Integration;
using UnityCommander.CLI.Mode;
using UnityCommander.Commands.Diagnostic;
using UnityCommander.Commands.Parsing;
using UnityCommander.Commands.Services;
using UnityCommander.Common.Diagnostic;
using UnityCommander.Diagnostics.Diagnostic;


namespace UnityCommander.Commands
{
    [ConsoleCommand("inspect", "Выводит список открытых файлов указанного процесса по имени.", "procof")]
    public class InspectCommand : IConsoleCommand
    {
        private IDiagnosticRender _renderer;
        private ICommandArgumentParser _parser;
        private IDiagnosticPipeline _pipeline;
        
        private WatchService _watchService;
        
        private DiagnosticTrace _trace;

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
            _watchService = new WatchService();
        }

        public async Task ExecuteAsync(IConsoleCommandContext context, CancellationToken cancellationToken)
        {
            var output = context.Output;
            var writer = new DiagnosticConsoleWriter(output);

            var args = _parser.Parse(context.Arguments);

            var interval = args.GetInt("interval");
            var isWatch = args.HasFlag("watch");
            var isReporter = args.HasFlag("report");
            var isTrace = args.HasFlag("trace");

            var query = new DiagnosticQuery
            {
                Source = args.GetAt(0),
                Format = args.GetString("format"),
            };

            if (!isWatch)
            {
                if (isReporter)
                    _pipeline.Report(query, writer);
                else
                    _renderer.Render(output, _pipeline.Execute(query));

                return;
            }
            await _watchService.Run(
                interval,
                () =>
                {
                    if (isReporter)
                    {
                        if (isTrace)
                        {
                            if (!_pipeline.ReportChanged(query, writer))
                                return;

                            //output.WriteLine(writer.ToString());
                            return;
                        }

                        output.Clear();

                        _pipeline.Report(query, writer);

                        return;
                    }

                    var result = _pipeline.Execute(query);

                    if (!isTrace)
                        output.Clear();

                    _renderer.Render(output, result);
                },
                cancellationToken);
        }

        public Task FinalizeAsync()
        {
            return Task.CompletedTask;
        }
    }
}
