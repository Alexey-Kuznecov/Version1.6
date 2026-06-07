
using UnityCommander.Autocomplete.Context;
using UnityCommander.Autocomplete.Infrastructure;

namespace UnityCommander.Autocomplete.Completion
{
    public interface ICompletionProvider
    {
        bool CanHandle(CliParseState context);
        IEnumerable<CompletionItem> GetCompletions(CliParseState context);
    }
}
