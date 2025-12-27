using UnityCommander.Autocomplete.Input;

namespace UnityCommander.Autocomplete.Completion
{
    public interface ICompletionEngine
    {
        CompletionResult GetCompletions(InputState state);

        TextEdit ApplyCompletion(
            InputState state,
            CompletionItem item);

        IReadOnlyList<InputToken> GetAllTokens();
        InputToken? GetTokenNearCaret(string text, int caretPosition);
    }
}
