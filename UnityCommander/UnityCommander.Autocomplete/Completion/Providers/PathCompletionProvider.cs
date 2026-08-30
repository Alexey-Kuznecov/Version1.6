
using UnityCommander.Abstractions.Completion;
using UnityCommander.Autocomplete.Infrastructure;

namespace UnityCommander.Autocomplete.Completion.Providers
{
    public class PathCompletionProvider : ICompletionProvider
    {
        public int Priority => 200;

        public bool CanHandle(CliParseState ctx)
            => ctx.ExpectedValue?.ValueType == ArgumentValueType.Path;

        public IEnumerable<CompletionItem> GetCompletions(
            CliParseState ctx)
            {
                if (ctx.ExpectedValue == null)
                    return Array.Empty<CompletionItem>();

                return new[]
                {
                new CompletionItem
                {
                    DisplayText = ctx.ExpectedValue.Descriptor.Name,
                    InsertText = "\"\"",
                    CaretOffset = -2
                }
            };
        }
    }
}
