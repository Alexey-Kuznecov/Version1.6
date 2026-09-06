
using UnityCommander.Autocomplete.Infrastructure;

namespace UnityCommander.Autocomplete.Completion
{
    public interface ICompletionProvider
    {
        int Priority { get; }
        bool CanHandle(CliParseState context);
        IEnumerable<CompletionItem> GetCompletions(CliParseState context);
    }
}
