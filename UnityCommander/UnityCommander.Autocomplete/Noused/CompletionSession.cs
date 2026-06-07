using UnityCommander.Autocomplete.Completion;

namespace UnityCommander.CLI.Input
{
    public sealed class CompletionSession
    {
        public IReadOnlyList<CompletionItem> Items { get; }
        public int SelectedIndex { get; set; }
        public bool IsActive => Items.Count > 0;

        public CompletionSession(IReadOnlyList<CompletionItem> items)
        {
            Items = items;
            SelectedIndex = items.Count - 1;
        }
    }
}
