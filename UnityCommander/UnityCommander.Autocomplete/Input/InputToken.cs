using UnityCommander.Autocomplete.Tokenization;

namespace UnityCommander.Autocomplete.Input
{
    public sealed class InputToken
    {
        public string? OriginalValue { get; init; }
        public string? CurrentValue { get; init; }
        public bool IsModified => !string.Equals(
           OriginalValue,
           CurrentValue,
           StringComparison.OrdinalIgnoreCase);

        public int Start { get; init; }
        public int Length { get; init; }
        public TokenKind Kind { get; init; }
        public int StartIndex { get; internal set; }
    }
}
