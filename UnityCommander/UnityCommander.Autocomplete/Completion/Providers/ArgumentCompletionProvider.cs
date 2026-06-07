
using UnityCommander.Abstractions.Completion;
using UnityCommander.Autocomplete.Infrastructure;

namespace UnityCommander.Autocomplete.Completion.Providers
{
    public class ArgumentCompletionProvider : ICompletionProvider
    {
        public bool CanHandle(CliParseState ctx)
            => ctx.ExpectedNext == CompletionKind.PositionalArgument;

        public IEnumerable<CompletionItem> GetCompletions(CliParseState ctx)
        {
            return ctx.AvailableArguments
                .Select(arg => new CompletionItem
                {
                    DisplayText = arg.Name,
                    InsertText = arg.Name
                });
        }
    }
}
