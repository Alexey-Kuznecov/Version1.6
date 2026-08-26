

using UnityCommander.Abstractions.Completion;
using UnityCommander.Autocomplete.Diagnostic;
using UnityCommander.Common.Diagnostic;
using UnityCommander.Logging.Contracts;
using UnityCommander.Logging.Core;
using UnityCommander.Logging.Extensions;
using UnityCommander.Logging.Infrastructure;

namespace UnityCommander.Autocomplete.Infrastructure.Analyze
{
    public sealed class CliInputAnalyzer : ICliInputAnalyzer, IDiagnosticReporter
    {
        private readonly IReadOnlyList<ICommandDescriptor> _commands;

        private InputDiagnostics? _diagnostics;

        private readonly ILogger? _logger;

        #region Report Data

        private string __text = string.Empty;
        private int __lastCaretPosition = 0;

        #endregion

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
            _logger?.Info($"text: {text} caretPosition:  {caretPosition}");

            var tokens = Tokenize(text);

            var snapshot = tokens
                .Select(t => t.Clone())
                .ToList();

            MarkActiveToken(tokens, caretPosition);

           _logger?.CollectionDiff("MarkActiveToken", snapshot, tokens,
           (log, oldToken, newToken) =>
           {
               if (oldToken.Status != newToken.Status)
               {
                   log.Warning(
                       $"{oldToken.Text}: {oldToken.Status} -> {newToken.Status}");
               }
           });

            var snapshot2 = tokens
                .Select(t => t.Clone())
                .ToList();

            var status = new InputStatus
            {
                Tokens = tokens,
                //ActiveToken = tokens.FirstOrDefault(t => t.Status == TokenStatus.Editing)
            };

            //_logger?.CollectionInfo($"TokenStatus {status?.ActiveToken?.Text}", tokens, t =>
            //{
            //    _logger.Info($"{t.Text}; = {t.Status} \n");
            //});

            ResolveTokens(status);

            _logger?.CollectionDiff("ResolveTokens", snapshot2, tokens,
            (log, oldToken, newToken) =>
            {
                if (oldToken.Status == newToken.Status)
                {
                    log.Warning(
                        $"{oldToken.Text}: {oldToken.Status} -> {newToken.Status}");
                }
            });

            var snapshot3 = tokens
                .Select(t => t.Clone())
                .ToList();

            var currentToken = tokens.FirstOrDefault(t => t.IsActive);

            _diagnostics = new InputDiagnostics
            {
                Text = text,
                CaretIndex = caretPosition,
                CurrentToken = currentToken?.Clone(),
                Tokens = tokens
                   .Select(t => t.Clone())
                   .ToList()
            };

            // 3. Валидация (пока можно stub)
            //ResolveValidation(status);

            // 4. Фаза + ожидания
            ResolveExpectedKind(status);

            _logger?.CollectionDiff("ResolveTokens", snapshot3, tokens,
            (log, oldToken, newToken) =>
            {
                log.Warning(
                    $"{oldToken.Text}: {oldToken.Status} -> {newToken.Status}");
            });

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
                
                if (token.IsActive)
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

                // Здесь надо проверить соответствие ожидаемому типу значения,
                // если FlagValue у тебя именно именованное значение.
                ctx.WaitingFlagValue = null;
                return;
            }

            if (ctx.Command == null)
            {
                token.Kind = TokenKind.Command;

                ctx.Command = ResolveCommand(token.Text);
                status.Command = ctx.Command;

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

                token.IsComplete = flag != null &&
                                   token.Text == flag.Name;

                if (flag?.RequiresValue == true)
                    ctx.WaitingFlagValue = flag;

                return;
            }

            token.Kind = TokenKind.PositionalArgument;
        }

        private ICommandVariant? ResolveVariant(ICommandDescriptor command, string name)
        {
            return command.Variants?
                .FirstOrDefault(v => v.Name.StartsWith(name, StringComparison.OrdinalIgnoreCase));
        }

        private IFlagDescriptor? ResolveFlag(AnalyzerContext ctx, string text)
        {
            if (ctx.Variant == null)
                return null;

            return ctx.Variant.Flags.FirstOrDefault(f =>
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
            if (active == null)
            {
                status.ActiveToken = CreateVirtualToken(status);
                status.ExpectedKind = ResolveNextExpected(status);
            }
        }

        private ExpectedKind ResolveNextExpected(InputStatus status)
        {
            // 1️⃣ Команда ещё не выбрана
            if (status.Command == null)
                return ExpectedKind.Command;

            // 2️⃣ Есть варианты — но вариант ещё не выбран
            if (status.Command.Variants.Any() && status.Variant == null)
                return ExpectedKind.Variant;

            var variant = status.Variant;
            if (variant == null)
                return ExpectedKind.Nothing;

            //// 3️⃣ Ожидается значение флага
            //if (status.Context.WaitingFlagValue != null)
            //    return ExpectedKind.FlagValue;

            //// 4️⃣ Позиционные аргументы
            //if (status.Context.PositionalIndex < variant.Arguments.Count)
            //    return ExpectedKind.PositionalArgument;

            // 5️⃣ Флаги (если есть)
            if (variant.Flags.Any())
                return ExpectedKind.Flag;

            // 6️⃣ Всё введено
            return ExpectedKind.Nothing;
        }

        private AnalyzerToken CreateVirtualToken(InputStatus status)
        {
            var last = status.Tokens.LastOrDefault();

            var start = last?.End +1 ?? 0;

            return new AnalyzerToken("", start)
            {
                IsActive = true,
                //IsVirtual = true
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
