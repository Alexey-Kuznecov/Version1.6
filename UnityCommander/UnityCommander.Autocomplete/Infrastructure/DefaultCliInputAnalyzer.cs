using UnityCommander.Autocomplete.Completion;
using UnityCommander.Autocomplete.Context.Descriptors;
using UnityCommander.Logging.Contracts;
using UnityCommander.Logging.Core;
using UnityCommander.Logging.Infrastructure;
using static System.Net.Mime.MediaTypeNames;

namespace UnityCommander.Autocomplete.Infrastructure
{
    public sealed class DefaultCliInputAnalyzer : ICliInputAnalyzer
    {
        private readonly IReadOnlyList<ICommandDescriptor> _commands;
        private readonly ILogger _logger;

        public DefaultCliInputAnalyzer(IReadOnlyList<ICommandDescriptor> commands, LoggerCreator loggerCreator)
        {
            _logger = loggerCreator.For<DefaultCliInputAnalyzer>(LogScope.UserAction);
            _commands = commands;
        }

        public CliParseState Analyze(string text, int caretPosition)
        {
            var tokens = Tokenize(text);
            _logger.CollectionInfo("Tokens:", tokens, 
                () => tokens.Count >= 2);
            
            if (tokens.Count == 0)
            {
                return Empty(CommandExpected());
            }

            // 1️⃣ Команда
            var commandToken = tokens[0];
            var command = _commands.FirstOrDefault(c =>
                c.Name.StartsWith(commandToken, StringComparison.OrdinalIgnoreCase));

            if (command == null)
            {
                return ErrorState(
                    CompletionKind.Command,
                    $"Unknown command '{commandToken}'"
                );
            }

            var positionalArgs = new List<ParsedArgument>();
            var flags = new List<ParsedFlag>();

            int argIndex = 0;
            int i = 1;

            // 2️⃣ Разбор остального
            while (i < tokens.Count)
            {
                var token = tokens[i];

                // Флаг
                if (token.StartsWith("-"))
                {
                    var flag = command.Flags.FirstOrDefault(f =>
                        f.Name == token || f.ShortName == token);
                    

                    if (flag == null)
                    {
                        return ErrorState(
                            CompletionKind.Flag,
                            $"Unknown flag '{token}'"
                        );
                    }

                    if (flag.RequiresValue)
                    {
                        if (i + 1 >= tokens.Count)
                        {
                            return new CliParseState(
                                command,
                                positionalArgs,
                                flags,
                                CompletionKind.FlagValue,
                                argIndex,
                                null
                            );
                        }
                        var nextToken = tokens[i + 1];

                        // 🔴 ВАЖНОЕ МЕСТО
                        if (nextToken.StartsWith("-"))
                        {
                            var nextFlag = command.Flags.FirstOrDefault(f =>
                                f.Name == nextToken || f.ShortName == nextToken);

                            if (nextFlag != null)
                            {
                                return ErrorState(
                                    CompletionKind.FlagValue,
                                    $"Flag '{flag.Name}' requires a value"
                                );
                            }
                        }

                        flags.Add(new ParsedFlag(flag, nextToken));
                        i += 2;
                    }
                    else
                    {
                        flags.Add(new ParsedFlag(flag, null));
                        i++;
                    }

                    continue;
                }

                // Позиционный аргумент
                if (argIndex >= command.PositionalArguments.Count)
                {
                    return ErrorState(
                        CompletionKind.Error,
                        "Too many positional arguments"
                    );
                }

                var descriptor = command.PositionalArguments[argIndex];
                positionalArgs.Add(new ParsedArgument(descriptor, token));
                argIndex++;
                i++;
            }

            // 3️⃣ Что дальше?
            if (argIndex < command.PositionalArguments.Count)
            {
                return new CliParseState(
                    command,
                    positionalArgs,
                    flags,
                    CompletionKind.PositionalArgument,
                    argIndex,
                    null
                );
            }

            return new CliParseState(
                command,
                positionalArgs,
                flags,
                CompletionKind.Flag,
                argIndex,
                null
            );
        }

        private CliParseState Empty(CliParseState cliParseState)
             => new(null, Array.Empty<ParsedArgument>(), Array.Empty<ParsedFlag>(), cliParseState.ExpectedNext, 0, null);

        // ===== helpers =====

        private static List<string> Tokenize(string text)
        {
            return text
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .ToList();
        }

        private static CliParseState Empty(CompletionKind next)
            => new(null, Array.Empty<ParsedArgument>(), Array.Empty<ParsedFlag>(), next, 0, null);

        private static CliParseState CommandExpected()
            => Empty(CompletionKind.Command);

        private static CliParseState ErrorState(
            CompletionKind next,
            string message)
            => new(null, Array.Empty<ParsedArgument>(), Array.Empty<ParsedFlag>(), next, 0,
                new CliError(message));
    }
}
