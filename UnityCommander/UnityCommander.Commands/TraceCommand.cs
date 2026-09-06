
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityCommander.CLI.Core;
using UnityCommander.CLI.Integration;
using UnityCommander.CLI.Mode;
using UnityCommander.Commands.Parsing;
using UnityCommander.Diagnostics.Tracing;

namespace UnityCommander.Commands
{
    [ConsoleCommand("trace", "Вывести диагностический trace")]
    public class TraceCommand : IConsoleCommand
    {
        private readonly IDiagnosticTraceStore _store;

        private ICommandArgumentParser _parser;

        public string Name => "trace";
        public string Description => "Вывести диагностический trace";

        public CommandExecutionMode Mode
            => CommandExecutionMode.Immediate;

        public TraceCommand(
            IDiagnosticTraceStore store, 
            ICommandArgumentParser parse)
        {
            _parser = parse;
            _store = store;
        }

        public async Task ExecuteAsync(
            IConsoleCommandContext context,
            CancellationToken cancellationToken)
        {
            var output = context.Output;

            var args = _parser.Parse(context.Arguments);

            IReadOnlyList<DiagnosticTraceEvent> entries;

            if (!args.HasFlag("source"))
            {
                entries = _store.GetSnapshot();
            }
            else
            {
                args.TryGetKeyValues("data", out var data);

                var query = new DiagnosticTraceQuery
                {
                    Source = args.GetString("source"),
                    Data = data.ToDictionary(
                        x => x.Key,
                        x => (object?)x.Value)
                };

                entries = _store.Query(query);
            }

            foreach (var entry in entries)
            {
                output.WriteLine(entry.ToString());
            }

            await Task.CompletedTask;
        }

        public Task FinalizeAsync()
        {
            return Task.CompletedTask;
        }
    }
}
