
using System.Linq;
using System.Xml.Linq;
using UnityCommander.Abstractions.Completion;
using UnityCommander.Logging.Contracts;
using UnityCommander.Logging.Core;
using UnityCommander.Logging.Infrastructure;

namespace UnityCommander.Autocomplete.Infrastructure.Analyze
{
    public sealed class CliInputAnalyzer : ICliInputAnalyzer
    {
        private readonly IReadOnlyList<ICommandDescriptor> _commands;
        private readonly ILogger? _logger;

        public CliInputAnalyzer(
            IReadOnlyList<ICommandDescriptor> commands,
            LoggerCreator? loggerCreator = null)
        {
            _commands = commands;
            _logger = loggerCreator?.For<DefaultCliInputAnalyzer>(LogScope.UserAction);
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

            _logger?.CollectionInfo($"TokenStatus {status?.ActiveToken?.Text}", tokens, t =>
            {
                _logger.Info($"{t.Text}; = {t.Status}");
            });

            ResolveTokens(status);

            // 3. Валидация (пока можно stub)
            //ResolveValidation(status);

            // 4. Фаза + ожидания
            ResolveExpectedKind(status);

            return status;
        }

        private void MarkActiveToken(
            IReadOnlyList<AnalyzerToken> tokens,
            int caretPosition)
        {
            foreach (var token in tokens)
            {
                if (IsCaretInsideToken(caretPosition, token))
                {
                    token.Status = TokenStatus.Editing;
                    token.IsComplete = false;
                    return;
                }

                token.Status = TokenStatus.Completed;
            }

            // Каретка после пробела → создаём виртуальный токен
            var last = tokens.LastOrDefault();
            if (last != null && caretPosition > last.End)
            {
                last.IsComplete = true;
            }
        }

        bool IsCaretInsideToken(int caret, AnalyzerToken token)
        {
            return caret >= token.Start &&
                   caret <= token.End + 1;
        }

        void ResolveTokens(InputStatus status)
        {
            var ctx = new AnalyzerContext();

            foreach (var token in status.Tokens)
            {
                ResolveToken(token, ctx, status);
                
                if (token.Status == TokenStatus.Editing)
                    break;
            }
        }

        void ResolveToken(
            AnalyzerToken token,
            AnalyzerContext ctx,
            InputStatus status)
        {
            // 1️⃣ Ожидается значение флага
            if (ctx.WaitingFlagValue != null)
            {
                token.Kind = TokenKind.FlagValue;
                ctx.WaitingFlagValue = null;
                return;
            }

            // 2️⃣ Команда ещё не выбрана
            if (ctx.Command == null)
            {
                token.Kind = TokenKind.Command;
                ctx.Command = ResolveCommand(token.Text);
                status.Command = ctx.Command;
                return;
            }

            // 3️⃣ Вариант (если есть)
            if (ctx.Variant == null && ctx.Command.Variants.Any())
            {
                token.Kind = TokenKind.Variant;
                ctx.Variant = ResolveVariant(ctx.Command, token.Text);
                status.Variant = ctx.Variant;
                return;
            }

            // 4️⃣ Флаг?
            if (token.Text.StartsWith("-"))
            {
                var flag = ResolveFlag(ctx, token.Text);
                token.Kind = TokenKind.Flag;

                if (flag?.RequiresValue == true)
                    ctx.WaitingFlagValue = flag;

                return;
            }

            // 5️⃣ Позиционный аргумент
            token.Kind = TokenKind.PositionalArgument;
            ctx.PositionalIndex++;
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
            // 1️⃣ Если есть редактируемый токен — он главный
            var editing = status.Tokens
                .LastOrDefault(t => t.Status == TokenStatus.Editing);

            if (editing != null)
            {
                status.ExpectedKind = editing.Kind switch
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

            // 2️⃣ Если НЕТ редактирования — смотрим, ожидается ли вообще что-то
            if (IsInputComplete(status))
            {
                status.ExpectedKind = ExpectedKind.Nothing;
                return;
            }

            // 3️⃣ Иначе — что ожидается следующим по контексту
            status.ExpectedKind = ResolveNextExpected(status);
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

        private bool IsInputComplete(InputStatus status)
        {
            if (status.Command == null)
                return false;

            if (status.Variant != null)
            {
                // TODO: тут позже учтёшь обязательные аргументы
                return true;
            }

            return false;
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
    }
}
