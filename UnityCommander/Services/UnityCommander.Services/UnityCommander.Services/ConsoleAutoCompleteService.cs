using System;
using System.Collections.Generic;
using System.Linq;
using UnityCommander.CLI.Autocomplete;
using UnityCommander.CLI.Helper;
using UnityCommander.CLI.Integration;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.Services
{
    public class ConsoleAutoCompleteService : IConsoleAutoComplete
    {
        private readonly ConsoleCommandDispatcher _dispatcher;

        public ConsoleAutoCompleteService(ConsoleCommandDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public IEnumerable<string> GetSuggestions(string input)
        {
            var parts = ParseHelper.ParseArguments(input);

            if (parts.Length == 0)
                return Enumerable.Empty<string>();

            // 👇 пока только имена команд
            if (parts.Length == 1)
            {
                return _dispatcher
                    .GetAvailableCommands()
                    .Select(c => c.Name)
                    .Where(n => n.StartsWith(parts[0], StringComparison.OrdinalIgnoreCase));
            }

            // 👇 аргументы — позже
            return Enumerable.Empty<string>();
        }
    }
}
