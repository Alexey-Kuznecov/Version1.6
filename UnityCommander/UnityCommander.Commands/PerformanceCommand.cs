using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityCommander.CLI.Core;
using UnityCommander.CLI.Integration;
using UnityCommander.CLI.Mode;
using UnityCommander.Commands.Parsing;
using UnityCommander.Diagnostics.Performance;

namespace UnityCommander.Commands
{
    [ConsoleCommand(
    "perf",
    "Диагностика производительности приложения.",
    "perf")]
    public sealed class PerformanceCommand : IConsoleCommand
    {
        private readonly ICommandArgumentParser _parser;
        private readonly IPerformanceAnalyzer _analyzer;
        private readonly IPerformanceProfiler _profiler;

        private readonly IPerformanceTableFormatter _formatter
            = new PerformanceTableFormatter();
        private IPerformanceSnapshotService _snapshotService;
        private IPerformanceComparisonService _comparisonService;
        private IPerformanceComparisonFormatter _comparisonFormatter 
            = new PerformanceComparisonFormatter();

        public string Name => "perf";

        public string Description =>
            "Диагностика производительности приложения.";

        public IEnumerable<string> Aliases =>
            ["performance"];

        public CommandExecutionMode Mode =>
            CommandExecutionMode.Immediate;

        public PerformanceCommand(
            ICommandArgumentParser parser,
            IPerformanceAnalyzer analyzer,
            IPerformanceProfiler profiler,
            IPerformanceSnapshotService snapshotService,
            IPerformanceComparisonService comparisonService)
        {
            _parser = parser;
            _analyzer = analyzer;
            _profiler = profiler;
            _snapshotService = snapshotService;
            _comparisonService = comparisonService;
        }

        public Task ExecuteAsync(
            IConsoleCommandContext context,
            CancellationToken cancellationToken)
        {
            var args = _parser.Parse(context.Arguments);

            var command = args.GetAt(0);

            switch (command?.ToLowerInvariant())
            {
                case "list":
                    List(context);
                    break;

                case "stats":
                    Stats(context, args);
                    break;

                case "slowest":
                    Slowest(context, args);
                    break;

                case "recent":
                    Recent(context, args);
                    break;

                case "snapshot":
                    CreateSnapshot(context, args);
                    break;

                case "compare":
                    Compare(context, args);
                    break;

                case "clear":
                    _profiler.Clear();

                    context.Output.WriteLine(
                        "Performance measurements cleared.");

                    break;

                default:
                    context.Output.WriteLine(
                        "Usage: perf <list|stats|slowest|clear> ...");

                    break;
            }

            return Task.CompletedTask;
        }

        public Task FinalizeAsync()
        {
            return Task.CompletedTask;
        }

        private void List(IConsoleCommandContext context)
        {
            foreach (var operation in _analyzer.GetOperations())
            {
                context.Output.WriteLine(operation);
            }
        }

        private void Stats(
            IConsoleCommandContext context,
            IArgumentCollection args)
        {
            var operation = args.GetAt(1);
            var flag = args.HasFlag("items");

            if (string.IsNullOrWhiteSpace(operation))
            {
                context.Output.WriteLine(
                    "Usage: perf stats <operation> [--items]");

                return;
            }

            if (flag)
            {
                StatsByItems(context, operation);

                context.Output.WriteLine(
                    "\n");
                return;
            }

            var stats =
                _analyzer.GetStatistics(operation);

            if (stats is null)
            {
                context.Output.WriteLine(
                    $"No measurements found for '{operation}'.");

                return;
            }

            context.Output.WriteLine(
                $"Performance: {stats.Operation}");

            context.Output.WriteLine(
                $"Samples : {stats.Count}");

            context.Output.WriteLine(
                $"Total   : {stats.Total.TotalMilliseconds:F2} ms");

            context.Output.WriteLine(
                $"Average : {stats.Average.TotalMilliseconds:F2} ms");

            context.Output.WriteLine(
                $"P95     : {stats.P95.TotalMilliseconds:F2} ms");

            context.Output.WriteLine(
                $"Min     : {stats.Min.TotalMilliseconds:F2} ms");

            context.Output.WriteLine(
                $"Max     : {stats.Max.TotalMilliseconds:F2} ms");

            context.Output.WriteLine(
                 "\n");
        }

        private void StatsByItems(
            IConsoleCommandContext context,
            string operation)
        {
            var stats =
                _analyzer.GetStatisticsByItems(operation);

            if (stats.Count == 0)
            {
                context.Output.WriteLine(
                    $"No 'Items' measurements found for '{operation}'.");

                return;
            }

            foreach (var line in _formatter.Format(stats))
            {
                context.Output.WriteLine(line);
            }
        }

        private void Slowest(
            IConsoleCommandContext context,
            IArgumentCollection args)
        {
            var operation = args.GetAt(1);

            if (string.IsNullOrWhiteSpace(operation))
            {
                context.Output.WriteLine(
                    "Usage: perf slowest <operation> [count]");

                return;
            }

            var count = 10;

            var countArgument = args.GetAt(2);

            if (int.TryParse(countArgument, out var parsedCount) &&
                parsedCount > 0)
            {
                count = parsedCount;
            }

            var measurements =
                _analyzer.GetSlowest(
                    operation,
                    count);

            if (measurements.Count == 0)
            {
                context.Output.WriteLine(
                    $"No measurements found for '{operation}'.");

                return;
            }

            context.Output.WriteLine(
                $"Slowest {operation} measurements:");

            foreach (var line in _formatter.Format(measurements))
            {
                context.Output.WriteLine(line);
            }

            context.Output.WriteLine(
                "\n");
        }

        private void Recent(
            IConsoleCommandContext context,
            IArgumentCollection args)
        {
            var operation = args.GetAt(1);

            if (string.IsNullOrWhiteSpace(operation))
            {
                context.Output.WriteLine(
                    "Usage: perf recent <operation> [count]");

                return;
            }

            var count = 10;

            if (int.TryParse(args.GetAt(2), out var parsed) &&
                parsed > 0)
            {
                count = parsed;
            }

            var measurements =
                _analyzer.GetRecent(
                    operation,
                    count);

            if (measurements.Count == 0)
            {
                context.Output.WriteLine(
                    $"No measurements found for '{operation}'.");

                return;
            }

            context.Output.WriteLine(
                $"Recent {operation} measurements:");

            context.Output.WriteLine("");

            foreach (var line in _formatter.Format(measurements))
            {
                context.Output.WriteLine(line);
            }

            context.Output.WriteLine(
                "\n");
        }

        private int _navCounter;

        private void CreateSnapshot(
            IConsoleCommandContext context,
            IArgumentCollection args)
        {

            var name = args.GetAt(2);

            if (string.IsNullOrWhiteSpace(name))
            {
                context.Output.WriteLine(
                    "Usage: perf snapshot create <name>");

                return;
            }

            var snapshot =
                _snapshotService.Create(name);

            context.Output.WriteLine(
                $"Snapshot '{snapshot.Name}' created.");

            _navCounter++;

            if (_navCounter == 8)
            {
                _navCounter = 0;
                context.Output.WriteLine("Navigation counter reset 8");
            }
        }

        private void Compare(
            IConsoleCommandContext context,
            IArgumentCollection args)
        {
            var first = args.GetAt(1);
            var second = args.GetAt(2);

            if (string.IsNullOrWhiteSpace(first) ||
                string.IsNullOrWhiteSpace(second))
            {
                context.Output.WriteLine(
                    "Usage: perf compare <snapshot> <snapshot>");

                return;
            }

            var comparisons =
                _comparisonService.Compare(
                    first,
                    second);

            foreach (var line in
                     _comparisonFormatter.Format(first, second, comparisons))
            {
                context.Output.WriteLine(line);
            }
        }
    }
}
