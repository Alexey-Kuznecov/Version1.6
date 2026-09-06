

using UnityCommander.Abstractions.Completion;
using UnityCommander.Autocomplete.Diagnostic;
using UnityCommander.Common.Diagnostic;
using UnityCommander.Logging.Contracts;
using UnityCommander.Logging.Core;
using UnityCommander.Logging.Infrastructure;

namespace UnityCommander.Autocomplete.Infrastructure.Analyze
{
    public sealed class CliInputAnalyzer : ICliInputAnalyzer, IDiagnosticReporter
    {
        private readonly IReadOnlyList<ICommandDescriptor> _commands;

        private InputDiagnostics? _diagnostics;

        private readonly ILogger? _logger;

        public string Name => "cli-input-analyzer";

        public DiagnosticCardinality Cardinality 
            => DiagnosticCardinality.Single;

        public CliInputAnalyzer(
            IReadOnlyList<ICommandDescriptor> commands, 
            IDiagnosticRegistry diagnostic,
            LoggerCreator? loggerCreator = null)
        {
            diagnostic.Register(this);

            _commands = commands;
            _logger = loggerCreator?.For<CliInputAnalyzer>(LogScope.Runtime);
        }

        public CliInputAnalyzer(ICommandCatalog catalog)
        {
            _commands = catalog.GetAll().ToList();
        }

        public InputStatus Analyze(string text, int caretPosition)
        {
            var tokens = Tokenize(text);

            MarkActiveToken(tokens, caretPosition);

            var status = new InputStatus
            {
                Tokens = tokens,
                ActiveToken = tokens.FirstOrDefault(t => t.Status == TokenStatus.Editing)
            };

            ResolveTokens(status);

            var currentToken = tokens.FirstOrDefault(t => t.IsActive);

            ResolveExpectedKind(status);

            _diagnostics = new InputDiagnostics
            {
                Text = text,
                CaretIndex = caretPosition,
                CurrentToken = currentToken?.Clone(),
                Status = status,
                Tokens = tokens
               .Select(t => t.Clone())
               .ToList()
            };

            return status;
        }

        private void MarkActiveToken(
            IReadOnlyList<AnalyzerToken> tokens,
            int caretPosition)
        {
            for (int i = 0; i < tokens.Count; i++)
            {
                var token = tokens[i];

                token.IsActive = IsCaretInsideToken(caretPosition, token);
                token.SemanticIndex = i;

                if (token.IsActive)
                    return;
            }
        }

        bool IsCaretInsideToken(int caret, AnalyzerToken token)
        {
            return caret >= token.Start &&
                   caret <= token.End;
        }

        void ResolveTokens(InputStatus status)
        {
            var ctx = new AnalyzerContext();

            foreach (var token in status.Tokens)
            {
                ResolveToken(token, ctx, status);
                
                if (token.IsActive && !token.IsComplete)
                {
                    status.ActiveToken = token;
                    token.Status = TokenStatus.Editing;
                    break;
                }
            }
        }

        void ResolveToken(
            AnalyzerToken token,
            AnalyzerContext ctx,
            InputStatus status)
        {
            if (ctx.ExpectedValue != null)
            {
                var expected = ctx.ExpectedValue;

                token.Kind = expected.Kind switch
                {
                    CompletionKind.Flag =>
                        TokenKind.FlagValue,

                    CompletionKind.PositionalArgument =>
                        TokenKind.PositionalArgument,

                    _ =>
                        TokenKind.Unknown
                };

                token.IsComplete = !token.IsActive;
                token.IsValid = IsValidValue(token.Text, expected);

                ctx.ExpectedValue = null;
                status.ExpectedValue = null;

                return;
            }

            if (ctx.Command == null)
            {
                token.Kind = TokenKind.Command;

                ctx.Command = ResolveCommand(token.Text);
                status.Command = ctx.Command;

                status.IsValidCommand = ctx.Command != null;
                token.IsComplete = ctx.Command != null &&
                                   token.Text == ctx.Command.Name;

                return;
            }

            if (ctx.Variant == null && ctx.Command.Variants.Any())
            {
                token.Kind = TokenKind.Variant;

                ctx.Variant = ResolveVariant(ctx.Command, token.Text);
                status.Variant = ctx.Variant;

                token.IsComplete = ctx.Variant != null &&
                                   token.Text == ctx.Variant.Name;

                return;
            }

            if (token.Text.StartsWith("-"))
            {
                var flag = ResolveFlag(ctx, token.Text);

                token.Kind = TokenKind.Flag;

                if (flag != null)
                {
                    ctx.HasUsedFlags = true;
                    status.UsedFlags.Add(flag);
                }

                token.IsComplete = flag != null &&
                                   token.Text == flag.Name;

                if (flag?.RequiresValue == true)
                {
                    var expectedValue = new ExpectedValue(
                        flag,
                        CompletionKind.Flag,
                        flag.ValueType);

                    status.ExpectedValue = expectedValue;
                    ctx.ExpectedValue = expectedValue;
                }

                return;
            }

            if (CanAcceptPositionalArgument(ctx))
            {
                var arguments =
                    ctx.Variant?.Arguments ??
                    ctx.Command?.Arguments;

                var argument = arguments![ctx.PositionalIndex];

                token.Kind = TokenKind.PositionalArgument;
                token.IsComplete = !token.IsActive;

                var expectedValue = new ExpectedValue(
                    argument,
                    CompletionKind.PositionalArgument,
                    argument.ValueType);

                token.IsValid = IsValidValue(
                    token.Text,
                    expectedValue);

                ctx.PositionalIndex++;
                status.PositionalIndex = ctx.PositionalIndex;

                if (!status.AvailableArguments.Contains(argument))
                    status.AvailableArguments.Add(argument);

                if (token.IsActive)
                {
                    status.ExpectedValue = expectedValue;
                }

                return;
            }
        }

        private bool CanAcceptPositionalArgument(
            AnalyzerContext ctx)
        {
            var arguments = ctx.Variant?.Arguments ?? ctx.Command?.Arguments;

            if (arguments == null)
                return false;

            if (ctx.PositionalIndex >= arguments.Count)
                return false;

            var policy = ctx.Variant?.PositionalArgumentPolicy 
                ?? ctx.Command?.PositionalArgumentPolicy 
                ?? PositionalArgumentPolicy.None;

            return policy switch
            {
                PositionalArgumentPolicy.None =>
                    false,

                PositionalArgumentPolicy.AfterVariant =>
                    !ctx.HasUsedFlags,

                PositionalArgumentPolicy.Anywhere =>
                    true,

                _ => false
            };
        }

        private ICommandVariant? ResolveVariant(ICommandDescriptor command, string name)
        {
            return command.Variants?
                .FirstOrDefault(v => v.Name.StartsWith(name, StringComparison.OrdinalIgnoreCase));
        }

        private IFlagDescriptor? ResolveFlag(
          AnalyzerContext ctx,
          string text)
        {
            var flags = ctx.Variant?.Flags ?? ctx.Command?.Flags;

            if (flags == null)
                return null;

            return flags.FirstOrDefault(f =>
                f.Name.StartsWith(text, StringComparison.OrdinalIgnoreCase) ||
                (!string.IsNullOrEmpty(f.ShortName) &&
                 f.ShortName.StartsWith(text, StringComparison.OrdinalIgnoreCase)));
        }

        private ICommandDescriptor? ResolveCommand(string name)
        {
            return _commands.FirstOrDefault(
                c => c.Name.StartsWith(name, StringComparison.OrdinalIgnoreCase));
        }

        private void ResolveExpectedKind(InputStatus status)
        {
            var active = status.Tokens.FirstOrDefault(t => t.IsActive);

            if (active != null)
            {
                if (active.Kind == TokenKind.PositionalArgument)
                {
                    status.ExpectedKind = CompletionKind.PositionalArgument;

                    var arguments =
                        status.Variant?.Arguments ??
                        status.Command?.Arguments;

                    if (arguments != null &&
                        status.PositionalIndex > 0 &&
                        status.PositionalIndex <= arguments.Count)
                    {
                        var argument = arguments[status.PositionalIndex - 1];

                        status.ExpectedValue = new ExpectedValue(
                            argument,
                            CompletionKind.PositionalArgument,
                            argument.ValueType);
                    }

                    return;
                }

                if (!active.IsComplete)
                {
                    status.ExpectedKind = active.Kind switch
                    {
                        TokenKind.Command => CompletionKind.Command,
                        TokenKind.Variant => CompletionKind.Variant,
                        TokenKind.Flag => CompletionKind.Flag,
                        TokenKind.FlagValue => CompletionKind.FlagValue,
                        _ => CompletionKind.Nothing
                    };

                    return;
                }
            }

            status.ActiveToken = CreateVirtualToken(status);
            status.ExpectedKind = ResolveNextExpected(status);
        }

        private CompletionKind ResolveNextExpected(InputStatus status)
        {
            if (status.Command == null)
                return CompletionKind.Command;

            if (status.Command.Variants.Any() &&
                status.Variant == null)
            {
                return CompletionKind.Variant;
            }

            if (status.ExpectedValue != null)
                return CompletionKind.FlagValue;

            var arguments = status.Variant?.Arguments
                            ?? status.Command.Arguments;

            var flags = status.Variant?.Flags
                        ?? status.Command.Flags;

            if (status.PositionalIndex < arguments?.Count)
            {
                if (arguments != null &&
                    status.PositionalIndex < arguments.Count)
                {
                    var argument = arguments[status.PositionalIndex];

                    status.ExpectedValue = new ExpectedValue(
                        argument,
                        CompletionKind.PositionalArgument,
                        argument.ValueType);
                }

                return CompletionKind.PositionalArgument;
            }

            if (flags.Any(flag =>
                !status.UsedFlags.Any(used => used.Name == flag.Name)))
            {
                return CompletionKind.Flag;
            }

            return CompletionKind.Nothing;
        }

        private bool IsValidValue(
            string text,
            ExpectedValue expected)
        {
            return expected.ValueType switch
            {
                ArgumentValueType.String =>
                    !string.IsNullOrWhiteSpace(text),

                ArgumentValueType.Int =>
                    int.TryParse(text, out _),

                ArgumentValueType.Boolean =>
                    bool.TryParse(text, out _),

                ArgumentValueType.Path =>
                    !string.IsNullOrWhiteSpace(text),

                ArgumentValueType.Enum =>
                    !string.IsNullOrWhiteSpace(text),

                _ => false
            };
        }

        private AnalyzerToken CreateVirtualToken(InputStatus status)
        {
            var last = status.Tokens.LastOrDefault();

            var start = last?.End +1 ?? 0;

            return new AnalyzerToken("", start)
            {
                IsActive = true,
                IsVirtual = true
            };
        }

        private List<AnalyzerToken> Tokenize(string text)
        {
            var tokens = new List<AnalyzerToken>();
            int pos = 0;

            while (pos < text.Length)
            {
                while (pos < text.Length && char.IsWhiteSpace(text[pos]))
                    pos++;

                if (pos >= text.Length)
                    break;

                int start = pos;

                if (text[pos] == '"')
                {
                    pos++;

                    while (pos < text.Length)
                    {
                        if (text[pos] == '"')
                        {
                            pos++;
                            break;
                        }

                        pos++;
                    }
                }
                else
                {
                    while (pos < text.Length && !char.IsWhiteSpace(text[pos]))
                        pos++;
                }

                var tokenText = text.Substring(start, pos - start);
                tokens.Add(new AnalyzerToken(tokenText, start));
            }

            return tokens;
        }

        public void Report(IDiagnosticWriter writer)
        {
            writer.BeginTable("Analyzer");

            writer.Row("Input", _diagnostics?.Text);
            writer.Row("Caret", _diagnostics?.CaretIndex);

            writer.Row("CurrentToken", _diagnostics?.CurrentToken?.Text);
            writer.Row("SemanticIndex", _diagnostics?.CurrentToken?.SemanticIndex);
            writer.Row("Kind", _diagnostics?.CurrentToken?.Kind);
            writer.Row("Status", _diagnostics?.CurrentToken?.Status);
            writer.Row("Complete", _diagnostics?.CurrentToken?.IsComplete);

            writer.Row("Status.ExpectedKind", _diagnostics?.Status?.ExpectedKind);
            writer.Row("Status.PositionalIndex", _diagnostics?.Status?.PositionalIndex);
            writer.Row("Status.IsValidCommand", _diagnostics?.Status?.IsValidCommand);
            writer.Row("Status.ExpectedValueType", _diagnostics?.Status?.ExpectedValue?.ValueType);
            writer.Row("Status.ExpectedKind", _diagnostics?.Status?.ExpectedValue?.Kind);
            writer.Row("Status.ExpectedDescriptor", _diagnostics?.Status?.ExpectedValue?.Descriptor);
            writer.Row("Status.ExpectedDescriptorName", _diagnostics?.Status?.ExpectedValue?.Descriptor.Name);

            writer.EndTable();

            //writer.BeginTable("Tokens");

            //foreach (var token in _diagnostics?.Tokens ?? [])
            //{
            //    writer.Row(
            //        $"{token.SemanticIndex}: {token.Text}",
            //        $"{token.Kind}, {token.Status}");
            //}

            //writer.EndTable();
        }
    }
}
