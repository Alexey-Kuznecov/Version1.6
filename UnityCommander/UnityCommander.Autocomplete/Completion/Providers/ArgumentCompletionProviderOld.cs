
using UnityCommander.Autocomplete.Context;
using UnityCommander.Autocomplete.Input;

namespace UnityCommander.Autocomplete.Completion.Providers
{
    //public sealed class ArgumentCompletionProvider : ICompletionProvider
    //{
    //    private readonly ConsoleCommandDispatcher _dispatcher;

    //    public ArgumentCompletionProvider(ConsoleCommandDispatcher dispatcher)
    //    {
    //        _dispatcher = dispatcher;
    //    }

    //    public bool CanHandle(InputContext context) => context is ArgumentContext;

    //    public IEnumerable<CompletionItem> GetCompletions(InputContext context)
    //    {
    //        var argCtx = (ArgumentContext)context;

    //        if (!_dispatcher.TryGetCommand(argCtx.CommandName, out var cmd))
    //            return Enumerable.Empty<CompletionItem>();

    //        if (cmd is not IAutoCompleteArgumentsProvider argProvider)
    //            return Enumerable.Empty<CompletionItem>();

    //        return argProvider
    //            .GetArgumentSuggestions(argCtx.ExistingArguments.ToArray())
    //            .Where(a => a.StartsWith(argCtx.PartialArgument, StringComparison.OrdinalIgnoreCase))
    //            .Select(a => new CompletionItem
    //            {
    //                DisplayText = a,
    //                EditFactory = state => new TextEdit(
    //                    argCtx.ReplaceStart,
    //                    argCtx.ReplaceLength,
    //                    argCtx.CurrentToken,
    //                    a + " "
    //                )
    //            });
    //    }
    //}
}
