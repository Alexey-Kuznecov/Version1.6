using UnityCommander.Autocomplete.Input;

namespace UnityCommander.Autocomplete.Tokenization
{
    public interface ITokenRegistry
    {
        IReadOnlyList<InputToken> Tokens { get; }
        InputToken? GetTokenAtPosition(int position);
        public InputToken? GetTokenNearCaret(string text, int caretPosition);
        public void UpdateTokens(IEnumerable<InputToken> tokens);
    }
}
