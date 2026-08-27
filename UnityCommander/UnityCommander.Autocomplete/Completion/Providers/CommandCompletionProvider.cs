
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
        {
            if (ctx.ExpectedNext != CompletionKind.Command)
                return false;

            if (ctx.Command == null && ctx.CurrentToken == null)
            {
                // неизвестная команда
                return false;
            }

            return true;
        }

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
