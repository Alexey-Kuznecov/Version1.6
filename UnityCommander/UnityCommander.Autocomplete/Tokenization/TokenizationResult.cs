using UnityCommander.Autocomplete.Input;

namespace UnityCommander.Autocomplete.Tokenization
{
    public sealed class TokenizationResult
    {
        public required IReadOnlyList<InputToken>? Tokens { get; init; }
        public required int CaretPosition { get; init; }

        // 👇 ВОТ ОНИ
        public required InputToken? CurrentToken { get; init; }
        public required int CurrentTokenStart { get; init; }
        public required int CurrentTokenLength { get; init; }
        public required string? CurrentTokenValue { get; init; }
    }
}
