using UnityCommander.Autocomplete.Infrastructure;
using UnityCommander.Autocomplete.Input;

namespace UnityCommander.Autocomplete.Completion
{
    public interface ICompletionEngine
    {
        public CompletionResult GetCompletions(InputState state, CliParseState parseState);

        TextEdit ApplyCompletion(
            InputState state,
            CompletionItem item);

        IReadOnlyList<InputToken> GetAllTokens();
        InputToken? GetTokenNearCaret(string text, int caretPosition);
    }
}
