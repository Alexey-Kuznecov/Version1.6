using UnityCommander.Autocomplete.Context.Descriptors;

namespace UnityCommander.Autocomplete.Completion
{
    public sealed class CompletionContext
    {
        CompletionKind Kind { get; }
        ICommandDescriptor? Command { get; }
        int ArgumentIndex { get; }
        string FilterText { get; }
    }
}
