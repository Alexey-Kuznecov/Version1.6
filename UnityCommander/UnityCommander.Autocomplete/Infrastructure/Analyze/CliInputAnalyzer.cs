

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
            if (ctx.WaitingFlagValue != null)
            {
                token.Kind = TokenKind.FlagValue;

                token.IsComplete = !token.IsActive;
                token.IsValid = IsValidValue(token.Text, ctx.WaitingFlagValue);
                ctx.WaitingFlagValue = null;
                status.ExpectedFlagValue = null;
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
                    status.ExpectedFlagValue = flag;
                    ctx.WaitingFlagValue = flag;
                }

                return;
            }

            //if (token.Text.StartsWith("-"))
            //{
            //    var flag = ResolveFlag(ctx, token.Text);

            //    token.Kind = TokenKind.Flag;

            //    if (flag != null)
            //    {
            //        ctx.HasUsedFlags = true;
            //        status.UsedFlags.Add(flag);
            //    }

            //    token.IsComplete = flag != null &&
            //                       token.Text == flag.Name;

            //    if (flag?.RequiresValue == true)
            //    {
            //        if (flag.Separator == ValueSeparator.Equals)
            //        {
            //            var separatorIndex = token.Text.IndexOf('=');

            //            if (separatorIndex >= 0)
            //            {
            //                var flagText = token.Text[..separatorIndex];
            //                var valueText = token.Text[(separatorIndex + 1)..];

            //                // flagText → ищем флаг
            //                // valueText → значение
            //            }

            //            token.IsComplete = flag != null &&
            //                    token.Text == flag.Name + "=";

            //            if (token.IsComplete)
            //            {
            //                status.ExpectedFlagValue = flag;
            //                ctx.WaitingFlagValue = flag;

            //                return;
            //            }
            //        }

            //        status.ExpectedFlagValue = flag;
            //        ctx.WaitingFlagValue = flag;
            //    }

            //    return;
            //}

            if (CanAcceptPositionalArgument(ctx))
            {
                token.Kind = TokenKind.PositionalArgument;
                ctx.PositionalIndex++;

                status.PositionalIndex = ctx.PositionalIndex;
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


        //private IFlagDescriptor? ResolveFlag(
        //    AnalyzerContext ctx,
        //    string text)
        //{
        //    var flags = ctx.Variant?.Flags ?? ctx.Command?.Flags;

        //    if (flags == null)
        //        return null;

        //    return flags.FirstOrDefault(f =>
        //        string.Equals(
        //            f.Name,
        //            text,
        //            StringComparison.OrdinalIgnoreCase) ||
        //        (!string.IsNullOrEmpty(f.ShortName) &&
        //         string.Equals(
        //             f.ShortName,
        //             text,
        //             StringComparison.OrdinalIgnoreCase)));
        //}


        private ICommandDescriptor? ResolveCommand(string name)
        {
            return _commands.FirstOrDefault(
                c => c.Name.StartsWith(name, StringComparison.OrdinalIgnoreCase));
        }

        private void ResolveExpectedKind(InputStatus status)
        {
            var active = status.Tokens.FirstOrDefault(t => t.IsActive);

            // Каретка внутри незавершённого токена.
            if (active != null && !active.IsComplete)
            {
                status.ExpectedKind = active.Kind switch
                {
                    TokenKind.Command => ExpectedKind.Command,
                    TokenKind.Variant => ExpectedKind.Variant,
                    TokenKind.Flag => ExpectedKind.Flag,
                    TokenKind.FlagValue => ExpectedKind.FlagValue,
                    TokenKind.PositionalArgument => ExpectedKind.PositionalArgument,
                    _ => ExpectedKind.Nothing
                };

                return;
            }

            // 3️⃣ Иначе — что ожидается следующим по контексту
            status.ActiveToken = CreateVirtualToken(status);
            status.ExpectedKind = ResolveNextExpected(status);
        }

        private ExpectedKind ResolveNextExpected(InputStatus status)
        {
            if (status.Command == null)
                return ExpectedKind.Command;

            if (status.Command.Variants.Any() &&
                status.Variant == null)
            {
                return ExpectedKind.Variant;
            }

            if (status.ExpectedFlagValue != null)
                return ExpectedKind.FlagValue;

            var arguments = status.Variant?.Arguments
                            ?? status.Command.Arguments;

            var flags = status.Variant?.Flags
                        ?? status.Command.Flags;

            if (status.PositionalIndex < arguments?.Count)
                return ExpectedKind.PositionalArgument;

            if (flags.Any(flag =>
                !status.UsedFlags.Any(used => used.Name == flag.Name)))
            {
                return ExpectedKind.Flag;
            }

            return ExpectedKind.Nothing;
        }

        private bool IsValidValue(
            string text,
            IFlagDescriptor flag)
        {
            return flag.ValueType switch
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

                if (pos >= text.Length) break;

                int start = pos;
                while (pos < text.Length && !char.IsWhiteSpace(text[pos]))
                    pos++;

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
            writer.Row("Status.ExpectedFlagValue", _diagnostics?.Status?.ExpectedFlagValue);

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
