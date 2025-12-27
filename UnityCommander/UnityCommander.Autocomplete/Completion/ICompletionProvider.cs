using UnityCommander.Autocomplete.Context;

namespace UnityCommander.Autocomplete.Completion
{
    public interface ICompletionProvider
    {
        bool CanHandle(InputContext context);
        IEnumerable<CompletionItem> GetCompletions(InputContext context);
    }
}
