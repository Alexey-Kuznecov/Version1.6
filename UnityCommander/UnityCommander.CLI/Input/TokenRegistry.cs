
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

        public InputToken? GetTokenAtPosition(int position)
        {
            return _tokens.FirstOrDefault(t => position >= t.Start && position <= t.Start + t.Length);
        }

        public InputToken? GetTokenNearCaret(string text, int caretPosition)
        {
            if (_tokens.Count == 0)
                return null;

            int pos = caretPosition > text.Length ? caretPosition - 1 : caretPosition;

            // 1️⃣ Если каретка в конце строки — сдвигаемся внутрь текста
            if (pos >= text.Length)
                pos = text.Length;

            // 2️⃣ Пропускаем пробелы ВЛЕВО
            while (pos >= text.Length)
                if (text[pos -1] != ' ')//|| char.IsLetter(text[pos -1]))
                    pos--;

            // 3️⃣ Ищем токен, в который попали
            return _tokens.FirstOrDefault(t =>
                pos >= t.Start &&
                pos < t.Start + t.Length);
        }

        public IReadOnlyList<InputToken> Tokens => _tokens;
    }
}
