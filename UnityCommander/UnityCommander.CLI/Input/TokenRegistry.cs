
using Newtonsoft.Json.Linq;

namespace UnityCommander.CLI.Input
{
    public class TokenRegistry : ITokenRegistry
    {
        private readonly List<InputToken> _tokens = new();

        public void UpdateTokens(IEnumerable<InputToken> tokens)
        {
            _tokens.Clear();
            _tokens.AddRange(tokens);
        }

        public InputToken? GetTokenAtCaret(string text, int caretPos)
        {
            if (string.IsNullOrEmpty(text) || caretPos == 0)
                return null;

            int pos = caretPos - 1;
            while (pos >= 0 && !char.IsLetterOrDigit(text[pos]))
                pos--;

            if (pos < 0)
                return null;

            int start = pos;
            while (start > 0 && char.IsLetterOrDigit(text[start - 1]))
                start--;

            int length = pos - start + 1;

            return new InputToken
            {
                StartIndex = start,
                Length = length,
                CurrentValue = text.Substring(start, length)
            };
        }

        public InputToken? GetTokenAtPosition(int position)
        {
            return _tokens.FirstOrDefault(t => position >= t.Start && position <= t.Start + t.Length);
        }

        public InputToken? GetTokenNearCaret(string text, int caretPosition)
        {
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
