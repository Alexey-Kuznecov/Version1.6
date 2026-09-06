
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityCommander.CLI.Core;
using UnityCommander.CLI.Integration;
using UnityCommander.CLI.Mode;
using UnityCommander.Commands.Parsing;
using UnityCommander.Logging.Configuration;
using UnityCommander.Logging.Infrastructure;

namespace UnityCommander.Commands
{
    [ConsoleCommand(
      "logger",
      "Управляет настройками внутреннего логгера.",
      "logger")]
    public sealed class LoggerCommand : IConsoleCommand
    {
        private readonly ICommandArgumentParser _parser;
        private readonly ILoggingRuntimeControl _runtime;

        public string Name => "logger";

        public string Description =>
            "Управляет настройками внутреннего логгера.";

        public IEnumerable<string> Aliases => ["log"];

        public CommandExecutionMode Mode =>
            CommandExecutionMode.Immediate;

        public LoggerCommand(
            ICommandArgumentParser parser,
            ILoggingRuntimeControl runtime)
        {
            _parser = parser;
            _runtime = runtime;
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

                case "on":
                    Enable(context, args);
                    break;

                case "off":
                    Disable(context, args);
                    break;

                case "reset":
                    _runtime.Reset();
                    context.Output.WriteLine("Logging runtime filters reset.");
                    break;

                default:
                    context.Output.WriteLine(
                        "Usage: logger <list|on|off|reset> ...");
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
            context.Output.WriteLine("Disabled Levels:");

            if (_runtime.DisabledLevels.Count == 0)
            {
                context.Output.WriteLine("  <none>");
            }
            else
            {
                foreach (var level in _runtime.DisabledLevels)
                    context.Output.WriteLine($"  {level}");
            }

            context.Output.WriteLine("");
            context.Output.WriteLine("Disabled categories:");

            if (_runtime.DisabledCategories.Count == 0)
            {
                context.Output.WriteLine("  <none>");
            }
            else
            {
                ListCategories(context);
            }

            context.Output.WriteLine("");
            context.Output.WriteLine("Disabled scopes:");

            if (_runtime.DisabledScopes.Count == 0)
            {
                context.Output.WriteLine("  <none>");
            }
            else
            {
                foreach (var scope in _runtime.DisabledScopes)
                    context.Output.WriteLine($"  {scope}");
            }
        }

        private void Enable(
            IConsoleCommandContext context,
            IArgumentCollection args)
        {
            var type = args.GetAt(1);
            var value = args.GetAt(2);

            switch (type?.ToLowerInvariant())
            {
                case "category":
                    _runtime.EnableCategory(value);
                    context.Output.WriteLine(
                        $"Logger category '{value}' enabled.");
                    break;

                case "scope":
                    _runtime.EnableScope(value);
                    context.Output.WriteLine(
                        $"Logger scope '{value}' enabled.");
                    break;

                case "level":
                    if (!Enum.TryParse<LogLevel>(
                            value,
                            true,
                            out var level))
                    {
                        context.Output.WriteLine(
                            $"Unknown log level '{value}'.");
                        return;
                    }

                    _runtime.EnableLevel(level);

                    context.Output.WriteLine(
                        $"Logger level '{level}' enabled.");
                    break;

                default:
                    context.Output.WriteLine(
                        "Usage: logger on <category|scope|level> <value>");
                    break;
            }
        }

        private void Disable(
            IConsoleCommandContext context,
            IArgumentCollection args)
        {
            var type = args.GetAt(1);
            var value = args.GetAt(2);

            switch (type?.ToLowerInvariant())
            {
                case "category":
                    _runtime.DisableCategory(value);
                    context.Output.WriteLine(
                        $"Logger category '{value}' disabled.");
                    break;

                case "scope":
                    _runtime.DisableScope(value);
                    context.Output.WriteLine(
                        $"Logger scope '{value}' disabled.");
                    break;

                case "level":
                    if (!Enum.TryParse<LogLevel>(
                            value,
                            true,
                            out var level))
                    {
                        context.Output.WriteLine(
                            $"Unknown log level '{value}'.");
                        return;
                    }

                    _runtime.DisableLevel(level);

                    context.Output.WriteLine(
                        $"Logger level '{level}' disabled.");
                    break;

                default:
                    context.Output.WriteLine(
                        "Usage: logger off <category|scope|level> <value>");
                    break;
            }
        }

        private void ListCategories(IConsoleCommandContext context)
        {
            var categories = _runtime.DisabledCategories
                .OrderBy(x => x, StringComparer.OrdinalIgnoreCase);

            context.Output.WriteLine("Disabled categories:");
            context.Output.WriteLine("");

            foreach (var category in categories)
                context.Output.WriteLine($"  {category}");
        }
    }
}
