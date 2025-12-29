
using UnityCommander.Abstractions.Completion;
using UnityCommander.Autocomplete.Context.Descriptors;
using UnityCommander.Autocomplete.Input;
using UnityCommander.Logging.Contracts;
using UnityCommander.Logging.Core;
using UnityCommander.Logging.Infrastructure;

namespace UnityCommander.Autocomplete.Infrastructure
{
    public sealed class DefaultCliInputAnalyzer : ICliInputAnalyzer
    {
        private readonly IReadOnlyList<ICommandDescriptor> _commands;
        private readonly ILogger? _logger;
        private CompletionKind _expectedNext = CompletionKind.Command;
        private bool _isTokenCompleted;

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

            if (tokens.Count == 0)
                return Empty(CompletionKind.Nothing, caretPosition);

            var currentToken = GetCurrentToken(tokens, caretPosition);

            //if (currentToken.Start > currentToken.Length)
            //{
            //    var index = text.IndexOf(' ', currentToken.Start - currentToken.Length);
            //    if (index == -1)
            //        return StateForIncompleteToken(null, currentToken, _expectedNext);
            //}

            var caretPositionMin = int.Min(caretPosition - 1, currentToken.Start - currentToken.Length);

            _isTokenCompleted = caretPosition > 0 && text[caretPositionMin] == ' ';

            _logger?.Debug($"Команда еще вводится пользователем!", () => _isTokenCompleted);
            // ❗ ЕСЛИ ТОКЕН НЕ ЗАВЕРШЁН — НЕ ПЕРЕХОДИМ НА СЛЕДУЮЩИЙ УРОВЕНЬ
            if (!_isTokenCompleted)
                return StateForIncompleteToken(null, currentToken, _expectedNext);
            
            _logger?.Debug($"ТОКЕН был введен полностью! Переходим к вводу {_expectedNext.ToString()}", () => _isTokenCompleted);
            var command = MatchCommand(tokens[0]);
            if (command == null)
                return ErrorState(CompletionKind.Command, $"Unknown command '{tokens[0].CurrentValue}'", caretPosition);

            if (tokens.Count == 1)
                return StateForIncompleteToken(command, tokens[0], CompletionKind.Command);

            var variant = MatchVariant(command, tokens[1]);
            if (variant == null)
                return ErrorState(CompletionKind.Variant, $"Unknown subcommand '{tokens[1].CurrentValue}'", caretPosition);

            return ParseArgumentsAndFlags(tokens, 2, variant, command, currentToken);
        }

        // ─── Получаем текущий токен по caret ─────────────────
        private InputToken GetCurrentToken(IReadOnlyList<InputToken> tokens, int caretPosition)
        
        {
            // Ищем токен, в котором находится курсор
            var token = tokens.FirstOrDefault(t => caretPosition >= t.Start && caretPosition <= t.Start + t.Length);
            _logger?.Info($"Current token {token?.CurrentValue}");
            if (token != null)
                return token;

            // Курсор после последнего токена или в промежутке → создаём новый токен
            // Находим токен, после которого стоит курсор
            var previousToken = tokens.LastOrDefault(t => t.Start + t.Length <= caretPosition);

            int start = previousToken != null ? previousToken.Start + previousToken.Length + 1 : caretPosition;
            _logger?.Info($"Current token {token?.CurrentValue}");
            return new InputToken
            {
                Start = start,
                Length = 0,
                CurrentValue = ""
            };
        }

        // ─── Поиск команды ─────────────────
        private ICommandDescriptor? MatchCommand(InputToken token)
        {
            return _commands.FirstOrDefault(c =>
                c.Name.StartsWith(token?.CurrentValue, StringComparison.OrdinalIgnoreCase));
        }

        // ─── Поиск варианта ─────────────────
        private ICommandVariant? MatchVariant(ICommandDescriptor command, InputToken token)
        {
            return command.Variants.FirstOrDefault(v =>
                v.Name.StartsWith(token.CurrentValue, StringComparison.OrdinalIgnoreCase));
        }

        // ─── Формируем состояние, когда только команда ─────────────────
        private CliParseState StateForIncompleteToken(
           ICommandDescriptor? command,
           InputToken token,
           CompletionKind kind)
        {
            return new CliParseState(
                command: command,
                positionalArguments: Array.Empty<ParsedArgument>(),
                flags: Array.Empty<ParsedFlag>(),
                availableArguments: Array.Empty<SimplePositionalArgumentDescriptor>(),
                availableFlags: Array.Empty<SimpleFlagDescriptor>(),
                expectedNext: kind,
                argumentIndex: 0,
                error: null,
                replaceStart: token.Start,
                replaceLength: token.Length,
                partialValue: token.CurrentValue,
                isEditingToken: true);
        }

        // ─── Разбор аргументов и флагов ─────────────────
        private CliParseState ParseArgumentsAndFlags(
            IReadOnlyList<InputToken> tokens,
            int startIndex,
            ICommandVariant variant,
            ICommandDescriptor command,
            InputToken currentToken)
        {
            var positionalArgs = new List<ParsedArgument>();
            var flags = new List<ParsedFlag>();
            int argIndex = 0;
            int lastFlagIndex = -1;

            for (int i = startIndex; i < tokens.Count;)
            {
                var token = tokens[i];

                if (token.CurrentValue.StartsWith("-"))
                {
                    var flagParseResult = TryParseFlag(token, variant, flags, lastFlagIndex, caretPosition: currentToken.Start);
                    if (flagParseResult.IsError)
                        return flagParseResult.ErrorState;

                    if (flagParseResult.ConsumedTokens == 2)
                        i += 2;
                    else
                        i += 1;

                    lastFlagIndex = flagParseResult.LastFlagIndex;
                    continue;
                }

                // ─── Позиционный аргумент ─────
                if (argIndex >= variant.Arguments.Count)
                    return ErrorState(CompletionKind.Error, "Too many positional arguments", currentToken.Start);

                var argDescriptor = variant.Arguments[argIndex];

                // Необязательные аргументы можно пропустить, если следующий токен — флаг
                if (!argDescriptor.IsRequired && token.CurrentValue.StartsWith("-"))
                {
                    i++;
                    continue;
                }

                positionalArgs.Add(new ParsedArgument(argDescriptor, token.CurrentValue));
                argIndex++;
                i++;
            }

            var availableArguments = variant.Arguments.Skip(argIndex).ToList();
            var availableFlags = variant.Flags
                .Where(f => f.IsRepeatable || !flags.Any(p => _isTokenCompleted)).ToList();

            //bool allFlagsWithoutValue = availableFlags.All(f => !f.RequiresValue);
            var availableVariants = command.Variants.ToList();

            ICommandVariant? matchedVariant = null;

            // пробуем найти вариант по текущему токену
            if (currentToken != null)
            {
                matchedVariant = availableVariants
                    .FirstOrDefault(v => v.Name.StartsWith(tokens[1].CurrentValue, StringComparison.OrdinalIgnoreCase));
            }
            if (matchedVariant != null && availableVariants.Any() && tokens.Count == 2)
            {
                // есть варианты, но текущий токен их не выбрал → предлагаем варианты
                _expectedNext = CompletionKind.Variant;
            }
            else if (matchedVariant != null)
            {
                // вариант выбран, идём дальше к позиционным аргументам и флагам
                _expectedNext = availableArguments.Any()
                    ? CompletionKind.PositionalArgument
                    : availableFlags.Any()
                        ? CompletionKind.Flag
                        : CompletionKind.Nothing;
            }
            else
            {
                // fallback
                _expectedNext = CompletionKind.Nothing;
            }
            _logger?.Debug($"Ожидание ввода варианта Variant", () => _expectedNext == CompletionKind.Variant);
            _logger?.Debug($"Ожидание ввода аргумента Argument", () => _expectedNext == CompletionKind.PositionalArgument);
            _logger?.Debug($"Ожидание ввода флага Flag", () => _expectedNext == CompletionKind.Flag);
            _logger?.Debug($"Ожидание ввода значения флага FlagValue", () => _expectedNext == CompletionKind.FlagValue);
            _logger?.Debug($"Команда полностью введена None", () => _expectedNext == CompletionKind.Nothing);
            //CompletionKind expectedNext = availableArguments.Any()
            //    ? CompletionKind.PositionalArgument
            //    : availableFlags.Any()
            //        ? CompletionKind.Flag
            //        : CompletionKind.None;

            //CompletionKind expectedNext = availableArguments.Any(a => a.IsRequired)
            //  ? CompletionKind.PositionalArgument
            //  : availableFlags.Any()
            //      ? CompletionKind.Flag
            //      : (availableArguments.Count == 0 || allFlagsWithoutValue)
            //          ? CompletionKind.None
            //          : CompletionKind.PositionalArgument;

            return new CliParseState(
                command,
                positionalArgs,
                flags,
                availableArguments,
                availableFlags,
                _expectedNext,
                argIndex,
                null,
                replaceStart: currentToken.Start,
                replaceLength: currentToken.Length,
                partialValue: currentToken.CurrentValue);
        }

        // ─── Разбор одного флага ─────────────────
        private (bool IsError, CliParseState ErrorState, int ConsumedTokens, int LastFlagIndex) TryParseFlag(
            InputToken token,
            ICommandVariant variant,
            List<ParsedFlag> flags,
            int lastFlagIndex,
            int caretPosition)
        {
            var flag = variant.Flags.FirstOrDefault(
                f => f.Name.StartsWith(token.CurrentValue ?? throw new ArgumentException()) 
                || f.ShortName.StartsWith(token.CurrentValue));
            if (flag == null)
                return (true, ErrorState(CompletionKind.Flag, $"Unknown flag '{token.CurrentValue}'", caretPosition), 0, lastFlagIndex);

            int consumed = 1;

            if (flag.RequiresValue)
            {
                // Проверка следующего токена
                var valueTokenIndex = flags.Count + 1;
                // Тут можно добавить проверку bounds и Start/Length
                // Для простоты: если значение не передано — ожидаем FlagValue
                // Можно вернуть CliParseState с CompletionKind.FlagValue
                // ...
            }

            flags.Add(new ParsedFlag(flag, null)); // Пока без значения

            // Проверка StrictOrder
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
                    return (true, ErrorState(CompletionKind.Flag, $"Unknown flag '{token.CurrentValue}'", caretPosition), 0, lastFlagIndex);

                lastFlagIndex = currentIndex;
            }

            return (false, null!, consumed, lastFlagIndex);
        }

        // ─────────────────────────────────────────────
        // Helpers
        // ─────────────────────────────────────────────

        public List<InputToken> Tokenize(string text)
        {
            var tokens = new List<InputToken>();
            int i = 0;
            while (i < text.Length)
            {
                while (i < text.Length && text[i] == ' ')
                    i++;

                if (i >= text.Length) break;

                int start = i;
                while (i < text.Length && text[i] != ' ')
                    i++;

                int length = i - start;
                tokens.Add(new InputToken
                {
                    Start = start,
                    Length = length,
                    CurrentValue = text.Substring(start, length),
                    OriginalValue = text.Substring(start, length)
                });
            }
            return tokens;
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
