
using UnityCommander.Abstractions.Completion;
using UnityCommander.Autocomplete.Infrastructure;

namespace UnityCommander.Autocomplete.Completion.Providers
{
    public class ArgumentCompletionProvider : ICompletionProvider
    {
        public int Priority => 100;

        public bool CanHandle(CliParseState ctx)
        {
            if (ctx.ExpectedValue == null)
                return ctx.ExpectedNext == CompletionKind.PositionalArgument;

            return ctx.ExpectedValue.ValueType is
                ArgumentValueType.String or
                ArgumentValueType.Int;
        }

        public IEnumerable<CompletionItem> GetCompletions(CliParseState ctx)
        {
            if (ctx.ExpectedValue != null)
            {
                return ctx.ExpectedValue.ValueType switch
                {
                    ArgumentValueType.String =>
                        new[]
                        {
                    new CompletionItem
                    {
                        DisplayText = ctx.ExpectedValue.Descriptor.Name,
                        InsertText = "\"\"",
                        CaretOffset = -2
                    }
                        },

                    ArgumentValueType.Int =>
                        Array.Empty<CompletionItem>(),

                    _ =>
                        Array.Empty<CompletionItem>()
                };
            }

            return ctx.AvailableArguments
                .Select(arg => new CompletionItem
                {
                    DisplayText = arg.Name,
                    InsertText = arg.Name
                });
        }
    }
}
