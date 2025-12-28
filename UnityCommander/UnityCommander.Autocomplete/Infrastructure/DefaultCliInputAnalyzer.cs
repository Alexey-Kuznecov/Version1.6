using System.Text;
using UnityCommander.Abstractions.Completion;
using UnityCommander.Autocomplete.Context.Descriptors;
using UnityCommander.Logging.Contracts;
using UnityCommander.Logging.Core;
using UnityCommander.Logging.Infrastructure;

namespace UnityCommander.Autocomplete.Infrastructure
{
    public sealed class DefaultCliInputAnalyzer : ICliInputAnalyzer
    {
        private readonly IReadOnlyList<ICommandDescriptor> _commands;
        private readonly ILogger? _logger;

        public DefaultCliInputAnalyzer(
            IReadOnlyList<ICommandDescriptor> commands,
            LoggerCreator? loggerCreator = null)
        {
            _commands = commands;
            _logger = loggerCreator?.For<DefaultCliInputAnalyzer>(LogScope.UserAction);
        }

        public DefaultCliInputAnalyzer(ICommandCatalog catalog)
        {
            _commands = catalog.GetAll().ToList();
        }

        public CliParseState Analyze(string text, int caretPosition)
        {
            var tokens = Tokenize(text);
            //_logger.CollectionInfo("Tokens:", tokens);

            // ────────────────────────────────
            // 0️⃣ Пустой ввод
            // ────────────────────────────────
            if (tokens.Count == 0)
                return Empty(CompletionKind.Command);

            // ────────────────────────────────
            // 1️⃣ Команда
            // ────────────────────────────────
            var commandToken = tokens[0];

            var command = _commands.FirstOrDefault(c =>
                c.Name.StartsWith(commandToken, StringComparison.OrdinalIgnoreCase));

            if (command == null)
                return ErrorState(CompletionKind.Command, $"Unknown command '{commandToken}'");

            // Только команда введена → ждём вариант
            if (tokens.Count == 1)
            {
                return new CliParseState(
                    command,
                    Array.Empty<ParsedArgument>(),
                    Array.Empty<ParsedFlag>(),
                    Array.Empty<SimplePositionalArgumentDescriptor>(),
                    Array.Empty<SimpleFlagDescriptor>(),
                    CompletionKind.Variant,
                    0,
                    null);
            }

            // ────────────────────────────────
            // 2️⃣ Variant
            // ────────────────────────────────
            var variantToken = tokens[1];

            var variant = command.Variants.FirstOrDefault(v =>
                v.Name.StartsWith(variantToken, StringComparison.OrdinalIgnoreCase));

            if (variant == null)
                return ErrorState(CompletionKind.Variant, $"Unknown subcommand '{variantToken}'");


            var positionalArgs = new List<ParsedArgument>();
            var flags = new List<ParsedFlag>();

            int i = 2;          // после command + variant
            int argIndex = 0;   // индекс позиции в variant.Arguments
            int lastFlagIndex = -1; // для StrictOrder
            
            _logger?.CollectionInfo("Variant Flags:", variant.Flags.Select(f => f.Name).ToList());
            _logger?.CollectionInfo("Tokens:", tokens);

            // ────────────────────────────────
            // 3️⃣ Разбор аргументов и флагов
            // ────────────────────────────────
            while (i < tokens.Count)
            {
                var token = tokens[i];

                // ─── Флаг ─────────────────────
                if (token.StartsWith("-"))
                {
                    var flag = variant.Flags.FirstOrDefault(f =>
                        f.Name == token || f.ShortName == token);

                    if (flag == null)
                        return ErrorState(CompletionKind.Flag, $"Unknown flag '{token}'");

                    if (flag.RequiresValue)
                    {
                        if (i + 1 >= tokens.Count)
                        {
                            // Значение флага ещё не введено
                            return new CliParseState(
                                command,
                                positionalArgs,
                                flags,
                                variant.Arguments.Skip(argIndex).ToList(),
                                variant.Flags
                                    .Where(f => f.IsRepeatable || !flags.Any(p => p.Descriptor == f))
                                    .ToList(),
                                CompletionKind.FlagValue,
                                argIndex,
                                null);
                        }

                        var valueToken = tokens[i + 1];

                        if (valueToken.StartsWith("-"))
                            return ErrorState(CompletionKind.FlagValue, $"Flag '{flag.Name}' requires a value");

                        flags.Add(new ParsedFlag(flag, valueToken));
                        i += 2;
                    }
                    else
                    {
                        flags.Add(new ParsedFlag(flag, null));
                        i++;
                    }

                    // ─── StrictOrder проверка ─────
                    if (variant.FlagOrderPolicy == FlagOrderPolicy.StrictOrder)
                    {
                        int currentIndex = -1;
                        for (int j = 0; j < variant.Flags.Count; j++)
                        {
                            if (variant.Flags[j] == flag)
                            {
                                currentIndex = j;
                                break;
                            }
                        }

                        if (currentIndex == -1)
                            throw new InvalidOperationException("Flag not found in variant.Flags");

                        if (currentIndex < lastFlagIndex)
                            return ErrorState(CompletionKind.Flag, $"Flag '{flag.Name}' is out of order");

                        lastFlagIndex = currentIndex;
                    }

                    continue;
                }

                // ─── Позиционный аргумент ─────
                if (argIndex >= variant.Arguments.Count)
                    return ErrorState(CompletionKind.Error, "Too many positional arguments");

                var descriptor = variant.Arguments[argIndex];
                positionalArgs.Add(new ParsedArgument(descriptor, token));
                argIndex++;
                i++;
            }

            var availableArguments = variant.Arguments
                .Skip(argIndex)
                .ToList();

            var availableFlags = variant.Flags
                .Where(f => f.IsRepeatable || !flags.Any(p => p.Descriptor == f))
                .ToList();

            // ────────── добавляем политику ──────────
            if (variant.FlagOrderPolicy == FlagOrderPolicy.StrictOrder)
            {
                availableFlags = availableFlags
                    .Skip(lastFlagIndex + 1)
                    .Take(1)
                    .ToList();
            }
            else if (variant.FlagOrderPolicy == FlagOrderPolicy.AfterPositionalArguments && argIndex < variant.Arguments.Count)
            {
                availableFlags = new List<IFlagDescriptor>();
            }

            // ────────────────────────────────
            // 5️⃣ Что ожидаем дальше
            // ────────────────────────────────
            CompletionKind expectedNext;

            if (availableArguments.Any())
                expectedNext = CompletionKind.PositionalArgument;
            else if (availableFlags.Any())
                expectedNext = CompletionKind.Flag;
            else
                expectedNext = CompletionKind.None;

            return new CliParseState(
                command,
                positionalArgs,
                flags,
                availableArguments,
                availableFlags,
                expectedNext,
                argIndex,
                null);
        }

        // ─────────────────────────────────────────────
        // Helpers
        // ─────────────────────────────────────────────

        private static List<string> Tokenize(string text)
        {
            var tokens = new List<string>();
            var sb = new StringBuilder();
            bool inQuotes = false;

            for (int i = 0; i < text.Length; i++)
            {
                var ch = text[i];

                if (ch == '"')
                {
                    inQuotes = !inQuotes;
                    continue;
                }

                if (char.IsWhiteSpace(ch) && !inQuotes)
                {
                    if (sb.Length > 0)
                    {
                        tokens.Add(sb.ToString());
                        sb.Clear();
                    }
                    continue;
                }

                sb.Append(ch);
            }

            if (sb.Length > 0)
                tokens.Add(sb.ToString());

            return tokens;
        }

        private static CliParseState Empty(CompletionKind next) =>
            new(null,
                Array.Empty<ParsedArgument>(),
                Array.Empty<ParsedFlag>(),
                   Array.Empty<SimplePositionalArgumentDescriptor>(),
                    Array.Empty<SimpleFlagDescriptor>(),
                next,
                0,
                null);

        private static CliParseState ErrorState(
            CompletionKind next,
            string message) =>
            new(null,
                Array.Empty<ParsedArgument>(),
                Array.Empty<ParsedFlag>(),
                   Array.Empty<SimplePositionalArgumentDescriptor>(),
                    Array.Empty<SimpleFlagDescriptor>(),
                next,
                0,
                new CliError(message));
    }
}
