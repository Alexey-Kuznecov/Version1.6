using UnityCommander.Autocomplete.Input;

namespace UnityCommander.Autocomplete.Completion
{
    public sealed class CompletionItem
    {
        public string? DisplayText { get; init; }
        public CompletionKind Kind { get; init; }
        public Func<InputState, TextEdit> EditFactory { get; set; } = null!;
    }
}
