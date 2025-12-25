
namespace UnityCommander.CLI.Input
{
    public class CompletionResult
    {
        public IReadOnlyList<CompletionItem>? Items { get; init; }
        public int DefaultSelectedIndex { get; init; }
        public bool HasCompletions { get; }
        public CompletionResult(IReadOnlyList<CompletionItem> items)
        {
            Items = items;
            HasCompletions = items.Count > 0;
        }
    }
}
