
namespace UnityCommander.CLI.Input
{
    public interface ICompletionEngine
    {
        CompletionResult GetCompletions(InputState state);

        TextEdit ApplyCompletion(
            InputState state,
            CompletionItem item);

        IReadOnlyList<InputToken> GetAllTokens();
        InputToken? GetTokenNearCaret(string text, int caretPosition);
        InputToken? GetTokenAtCaret(string text, int caretPos);
    }
}
