
namespace UnityCommander.CLI.Input
{
    public interface ICompletionEngine
    {
        CompletionResult GetCompletions(InputState state);

        TextEdit ApplyCompletion(
            InputState state,
            CompletionItem item);
    }
}
