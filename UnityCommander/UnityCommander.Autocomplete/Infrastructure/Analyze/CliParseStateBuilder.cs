
using Newtonsoft.Json.Linq;
using UnityCommander.Abstractions.Completion;
using UnityCommander.Autocomplete.Context.Descriptors;
using UnityCommander.Common.Diagnostic;

namespace UnityCommander.Autocomplete.Infrastructure.Analyze
{
    public sealed class CliParseStateBuilder : ICliParseStateBuilder, IDiagnosticReporter
    {
        public string Name => "cli.parse.state.builder";

        public InputStatus? Status = null;

        public DiagnosticCardinality Cardinality 
            => DiagnosticCardinality.Single;

        public CliParseStateBuilder(IDiagnosticRegistry diagnostic)
        {
            diagnostic.Register(this);
        }

        public CliParseState Build(InputStatus status)
        {
            Status = status;
            var command = status.Command;
            var variant = status.Variant;
            var tokens = status.Tokens ?? Array.Empty<AnalyzerToken>();
            var activeToken = status.ActiveToken;

            // Если команда ещё не выбрана
            var hasVariants = command?.Variants.Count > 0;

            if (hasVariants && variant == null)
            {
                var variantText = tokens.Count > 1
                    ? tokens[1].Text
                    : string.Empty;

                return CreateBaseState(
                    command,
                    null,
                    new CliError($"Unknown variant '{variantText}'"),
                    status,
                    activeToken);
            }

            // -------------------------
            // Позиционные аргументы
            // -------------------------

            var arguments = variant?.Arguments ?? command?.Arguments;

            var positionalTokens = tokens
                .Where(t => t.Kind == TokenKind.PositionalArgument)
                .ToList();

            var parsedArguments = new List<ParsedArgument>();

            for (int i = 0; i < positionalTokens.Count; i++)
            {
                if (i >= arguments?.Count)
                {
                    return ErrorState(
                        CompletionKind.Error,
                        "Too many positional arguments",
                        positionalTokens[i].Start);
                }

                var descriptor = arguments[i];

                parsedArguments.Add(
                    new ParsedArgument(
                        descriptor,
                        positionalTokens[i].Text));
            }

            // -------------------------
            // Флаги
            // -------------------------

            var flags = variant?.Flags ?? command?.Flags;

            var parsedFlags = new List<ParsedFlag>();

            foreach (var token in tokens)
            {
                if (token.Kind != TokenKind.Flag)
                    continue;

                if (token == activeToken ||
                    token.Status == TokenStatus.Editing)
                {
                    parsedFlags.Add(new ParsedFlag(null, null));
                    continue;
                }

                var flag = flags.FirstOrDefault(f =>
                    f.Name == token.Text ||
                    (!string.IsNullOrEmpty(f.ShortName) &&
                     f.ShortName == token.Text));

                if (flag == null)
                {
                    return ErrorState(
                        CompletionKind.Flag,
                        $"Unknown flag '{token.Text}'",
                        token.Start);
                }

                parsedFlags.Add(new ParsedFlag(null, null));
            }

            // -------------------------
            // Доступные позиционные аргументы
            // -------------------------

            var availableArguments = new List<SimplePositionalArgumentDescriptor>();

            if (status.PositionalIndex < arguments?.Count)
            {
                if (variant?.IsStrictOrder ?? command.IsStrictOrder)
                {
                    if (arguments[status.PositionalIndex] is SimplePositionalArgumentDescriptor next)
                        availableArguments.Add(next);
                }
                else
                {
                    for (int i = status.PositionalIndex; i < arguments.Count; i++)
                    {
                        if (arguments[i] is SimplePositionalArgumentDescriptor argument)
                            availableArguments.Add(argument);
                    }
                }
            }

            // -------------------------
            // Доступные флаги
            // -------------------------

            var availableFlags = new List<SimpleFlagDescriptor>();

            if (flags == null)
                flags = Array.Empty<SimpleFlagDescriptor>();

            foreach (var flag in flags)
            {
                if (flag.IsRepeatable || !status.UsedFlags.Contains(flag))
                {
                    if (flag is SimpleFlagDescriptor simpleFlag)
                        availableFlags.Add(simpleFlag);
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

        public void Report(IDiagnosticWriter writer)
        {
            writer.BeginTable("ActiveToken");

            writer.Row("Input", Status?.ActiveToken?.Text);

            writer.Row("CurrentToken", Status?.ActiveToken?.Text);
            writer.Row("SemanticIndex", Status?.ActiveToken?.SemanticIndex);
            writer.Row("Kind", Status?.ActiveToken?.Kind);
            writer.Row("Status", Status?.ActiveToken?.Status);
            writer.Row("Complete", Status?.ActiveToken?.IsComplete);

            writer.EndTable();

            writer.BeginTable("InputStatus");

            writer.Row("CommandName", Status?.Command?.Name);

            writer.Row("IsValidCommand", Status?.IsValidCommand);
            writer.Row("VariantName", Status?.Variant?.Name);
            writer.Row("ExpectedKind", Status?.ExpectedKind);
            writer.Row("TokensCount", Status?.Tokens?.Count);

            writer.EndTable();
        }
    }
}
