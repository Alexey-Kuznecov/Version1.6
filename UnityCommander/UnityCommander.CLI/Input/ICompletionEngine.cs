
namespace UnityCommander.CLI.Input
{
    public interface ICompletionEngine
    {
        CompletionResult GetCompletions(InputState state);

        TextEdit ApplyCompletion(
            InputState state,
            CompletionItem item);

        IReadOnlyList<InputToken> GetAllTokens();
        public InputToken? GetTokenNearCaret(string text, int caretPosition);
    }
}
