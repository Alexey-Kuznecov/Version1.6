
using UnityCommander.CLI.Core;

namespace UnityCommander.CLI.Autocomplete
{
    public class CommandAutoCompleteProvider : IAutoCompleteProvider
    {
        private readonly IConsoleCommandDispatcher _dispatcher;

        public CommandAutoCompleteProvider(IConsoleCommandDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public IEnumerable<string> GetSuggestions(string input)
        {
            return _dispatcher
                .GetAvailableCommands()
                .Select(c => c.Name)
                .Where(name => name.StartsWith(input, StringComparison.OrdinalIgnoreCase))
                .Distinct(); // Убираем дубли
        }
    }
}
