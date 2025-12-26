
using UnityCommander.Logging.Abstractions;

namespace UnityCommander.CLI.Input
{
    public class TokenRegistry : ITokenRegistry
    {
        private readonly List<InputToken> _tokens = new();
        private readonly ILogger? _appLogger;

        public TokenRegistry(ILogger? appLogger = null)
        {
            _appLogger = appLogger;
        }

        public void UpdateTokens(IEnumerable<InputToken> tokens)
        {
            _tokens.Clear();
            _tokens.AddRange(tokens);
        }

        public InputToken? GetTokenAtPosition(int position)
        {
            return _tokens.FirstOrDefault(t => position >= t.Start && position <= t.Start + t.Length);
        }

        public InputToken? GetTokenNearCaret(string text, int caretPosition)
        {
            _appLogger?.Info($"GetTokenNearCaret called with caretPos: {caretPosition} on text: '{text}'");
            if (_tokens.Count == 0 || string.IsNullOrEmpty(text))
                return null;

            int pos = caretPosition;

            if (pos >= text.Length)
                pos = text.Length - 1;

            if (pos < 0)
                return null;

            // 🔴 КЛЮЧЕВОЕ ПРАВИЛО:
            // если каретка на пробеле — токена нет
            if (text[pos] == ' ')
                return null;

            // (опционально) этот while теперь не нужен,
            // но можно оставить, если ты позже расширишь логику
            while (pos >= 0 && text[pos] == ' ')
                pos--;

            if (pos < 0)
                return null;

            return _tokens.FirstOrDefault(t =>
                pos >= t.Start &&
                pos < t.Start + t.Length);
        }

        public IReadOnlyList<InputToken> Tokens => _tokens;
    }
}
