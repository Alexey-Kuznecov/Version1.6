using UnityCommander.Abstractions.Completion;
using UnityCommander.Autocomplete.Context.Descriptors;

namespace UnityCommander.Autocomplete.Infrastructure.Analyze
{
    public sealed class CliParseStateBuilder : ICliParseStateBuilder
    {
        public CliParseState Build(InputStatus status)
        {
            var command = status.Command;
            var variant = status.Variant;
            var tokens = status.Tokens ?? Array.Empty<AnalyzerToken>();
            var activeToken = status.ActiveToken;

            // Если команда ещё не выбрана
            if (command == null)
            {
                return CreateBaseState(
                    command: null,
                    variant: null,
                    new CliError($"Unknown command '{tokens[0]?.Text}'"),
                    status,
                    activeToken
                );
            }

            if (tokens.Count == 1)
            {
                return CreateBaseState(
                    command,
                    null,
                    null,
                    status,
                    activeToken);
            }
            
            if (variant == null)
            {
                return CreateBaseState(
                    command: null,
                    variant: null,
                    new CliError($"Unknown command '{tokens[1].Text}'"),
                    status,
                    activeToken
                );
            }

            // -------------------------
            // Позиционные аргументы
            // -------------------------
            var positionalTokens = tokens
                .Where(t => t.Kind == TokenKind.PositionalArgument)
                .ToList();

            var parsedArguments = new List<ParsedArgument>();

            for (int i = 0; i < positionalTokens.Count; i++)
            {
                if (i >= variant.Arguments.Count)
                {
                     return ErrorState(CompletionKind.Error, "Too many positional arguments", positionalTokens[i].Start);
                }

                var descriptor = variant.Arguments[i];
                parsedArguments.Add(new ParsedArgument(descriptor, positionalTokens[i].Text));
            }

            // -------------------------
            // Флаги
            // -------------------------

            var parsedFlags = new List<ParsedFlag>();

            foreach (var token in tokens)
            {
                if (token.Kind != TokenKind.Flag)
                    continue;

                // ⚠️ Активный / редактируемый флаг — НЕ ВАЛИДИРУЕМ
                if (token == activeToken || token.Status == TokenStatus.Editing)
                {
                    parsedFlags.Add(new ParsedFlag(null, null));
                    continue;
                }

                var flag = variant.Flags.FirstOrDefault(f =>
                    f.Name == token.Text ||
                    !string.IsNullOrEmpty(f.ShortName) && f.ShortName == token.Text);

                if (flag == null)
                {
                    return ErrorState(CompletionKind.Flag, $"Unknown flag '{token.Text}'", token.Start);
                }

                parsedFlags.Add(new ParsedFlag(null, null));
                continue;
            }

            // -------------------------
            // Доступные позиционные аргументы
            // -------------------------

            var availableArguments = new List<SimplePositionalArgumentDescriptor>();

            int consumed = parsedArguments.Count;

            if (variant.IsStrictOrder)
            {
                if (consumed < variant.Arguments.Count &&
                    variant.Arguments[consumed] is SimplePositionalArgumentDescriptor next)
                {
                    availableArguments.Add(next);
                }
            }
            else
            {
                for (int i = consumed; i < variant.Arguments.Count; i++)
                {
                    if (variant.Arguments[i] is SimplePositionalArgumentDescriptor arg)
                    {
                        availableArguments.Add(arg);
                    }
                }
            }

            // -------------------------
            // Доступные флаги
            // -------------------------

            var availableFlags = new List<SimpleFlagDescriptor>();

            foreach (var flag in variant.Flags)
            {
                bool alreadyUsed = parsedFlags.Any(f => f.Descriptor == flag);

                if (!alreadyUsed || flag.IsRepeatable)
                {
                    if (flag is SimpleFlagDescriptor simpleFlag)
                    {
                        availableFlags.Add(simpleFlag);
                    }
                }
            }

            // -------------------------
            // Финальный CliParseState
            // -------------------------

            return new CliParseState(
                command: command,
                positionalArguments: parsedArguments,
                flags: parsedFlags,
                availableArguments: availableArguments,
                availableFlags: availableFlags,
                expectedNext: MapExpectedKind(status.ExpectedKind),
                argumentIndex: parsedArguments.Count,
                error: null,
                replaceStart: activeToken?.Start ?? 0,
                replaceLength: activeToken?.Length ?? 0,
                partialValue: activeToken?.Text ?? string.Empty
            );
        }

        private static CliParseState CreateBaseState(
            ICommandDescriptor? command,
            ICommandVariant? variant,
            CliError? error,
            InputStatus status,
            AnalyzerToken? activeToken)
        {
            return new CliParseState(
                command: command,
                positionalArguments: Array.Empty<ParsedArgument>(),
                flags: Array.Empty<ParsedFlag>(),
                availableArguments: Array.Empty<SimplePositionalArgumentDescriptor>(),
                availableFlags: Array.Empty<SimpleFlagDescriptor>(),
                expectedNext: MapExpectedKind(status.ExpectedKind),
                argumentIndex: 0,
                error: error,
                replaceStart: activeToken?.Start ?? 0,
                replaceLength: activeToken?.Length ?? 0,
                partialValue: activeToken?.Text ?? string.Empty
            );
        }

        private static CompletionKind MapExpectedKind(ExpectedKind kind)
        {
            return kind switch
            {
                ExpectedKind.Command => CompletionKind.Command,
                ExpectedKind.Variant => CompletionKind.Variant,
                ExpectedKind.Flag => CompletionKind.Flag,
                ExpectedKind.PositionalArgument => CompletionKind.PositionalArgument,
                ExpectedKind.FlagValue => CompletionKind.FlagValue,
                ExpectedKind.Nothing => CompletionKind.Nothing,
                _ => CompletionKind.Nothing
            };
        }

        private static CliParseState Empty(CompletionKind next, int caretPosition) =>
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
            string message,
            int caretPosition) =>
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
