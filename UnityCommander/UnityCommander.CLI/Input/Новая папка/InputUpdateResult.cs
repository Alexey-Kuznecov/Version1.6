
namespace UnityCommander.CLI.Input
{
    public sealed class InputUpdateResult
    {
        public IReadOnlyList<CompletionItem>? Suggestions { get; init; }
        public int SelectedIndex { get; init; }

        public TextEdit? AppliedEdit { get; init; }
    }
}
