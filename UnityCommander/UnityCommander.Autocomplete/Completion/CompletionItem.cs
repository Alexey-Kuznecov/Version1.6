using UnityCommander.Abstractions.Completion;
using UnityCommander.Autocomplete.Input;
using UnityCommander.Logging.Contracts;

namespace UnityCommander.Autocomplete.Completion
{
    public sealed class CompletionItem
    {
        public string? InsertText { get; init; }

        public string? DisplayText { get; init; }

        public CompletionKind Kind { get; init; }

        public Func<InputState, TextEdit> EditFactory { get; set; } = null!;

        public ILogger? Logger { get; set; }
    }
}
