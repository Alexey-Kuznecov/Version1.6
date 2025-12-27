using UnityCommander.Autocomplete.Input;

namespace UnityCommander.Autocomplete.Tokenization
{
    public sealed class TokenizationResult
    {
        public IReadOnlyList<InputToken>? Tokens { get; init; }
        public int CaretPosition { get; init; }

        // 👇 ВОТ ОНИ
        public InputToken? CurrentToken { get; init; } = new ();
        public int CurrentTokenStart { get; init; }
        public int CurrentTokenLength { get; init; }
        public string? CurrentTokenValue { get; init; }
    }
}
