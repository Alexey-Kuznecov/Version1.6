
using UnityCommander.Abstractions.Completion;
using UnityCommander.Autocomplete.Infrastructure;

namespace UnityCommander.Autocomplete.Completion.Providers
{
    public class FlagCompletionProvider : ICompletionProvider
    {
        public bool CanHandle(CliParseState ctx)
            => ctx.ExpectedNext == CompletionKind.Flag;

        public IEnumerable<CompletionItem> GetCompletions(CliParseState ctx)
        {
            return ctx.AvailableFlags
                .Select(f => new CompletionItem
                {
                    DisplayText = f.Name,
                    InsertText = f.Name
                });
        }
    }
}
