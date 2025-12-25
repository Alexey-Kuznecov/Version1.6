
using UnityCommander.CLI.Integration;

namespace UnityCommander.CLI.Input
{
    public sealed class CommandCompletionProvider : ICompletionProvider
    {
        private readonly ConsoleCommandDispatcher _dispatcher;

        public CommandCompletionProvider(ConsoleCommandDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public bool CanHandle(InputContext context) => context is CommandNameContext;

        public IEnumerable<CompletionItem> GetCompletions(InputContext context)
        {
            var cmdContext = context as CommandNameContext;
            if (cmdContext == null) return Enumerable.Empty<CompletionItem>();

            return _dispatcher
                .GetAvailableCommands()
                .Where(c => c.Name.StartsWith(cmdContext.PartialCommand, StringComparison.OrdinalIgnoreCase))
                .Select(c => new CompletionItem
                {
                    DisplayText = c.Name,
                    EditFactory = state =>
                    {
                        return new TextEdit(
                            cmdContext.ReplaceStart,
                            cmdContext.PartialCommand.Length,
                            cmdContext.CurrentToken,
                            c.Name + " "
                        );
                    }
                }); 
        }
    }
}
