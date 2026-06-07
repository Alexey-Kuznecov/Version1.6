
using UnityCommander.Abstractions.Completion;
using UnityCommander.Autocomplete.Infrastructure;

namespace UnityCommander.Autocomplete.Completion.Providers
{
    public class CommandCompletionProvider : ICompletionProvider
    {
        private readonly IReadOnlyList<ICommandDescriptor> _allCommands;

        public CommandCompletionProvider(IReadOnlyList<ICommandDescriptor> allCommands)
        {
            _allCommands = allCommands;
        }

        public bool CanHandle(CliParseState ctx)
            => ctx.ExpectedNext == CompletionKind.Command;

        public IEnumerable<CompletionItem> GetCompletions(CliParseState ctx)
        {
            // фильтруем команды по тому, что уже введено
            var partial = ctx.PartialValue ?? "";
            return _allCommands
                .Where(c => c.Name.StartsWith(partial, StringComparison.OrdinalIgnoreCase))
                .Select(cmd => new CompletionItem
                {
                    DisplayText = cmd.Name,
                    InsertText = cmd.Name
                });
        }
    }
}
