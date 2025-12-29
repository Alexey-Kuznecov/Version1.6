
using UnityCommander.Abstractions.Completion;
using UnityCommander.Autocomplete.Infrastructure;

namespace UnityCommander.Autocomplete.Completion.Providers
{
    public class VariantCompletionProvider : ICompletionProvider
    {
        public bool CanHandle(CliParseState ctx)
            => ctx.ExpectedNext == CompletionKind.Variant;

        public IEnumerable<CompletionItem> GetCompletions(CliParseState ctx)
        {
            // фильтруем команды по тому, что уже введено
            var partial = ctx.PartialValue ?? "";
            var command = ctx.Command ?? throw new ArgumentNullException();

            return command.Variants
                .Where(c => c.Name.StartsWith(partial, StringComparison.OrdinalIgnoreCase))
                .Select(cmd => new CompletionItem
                {
                    DisplayText = cmd.Name,
                    InsertText = cmd.Name
                });
        }
    }
}
