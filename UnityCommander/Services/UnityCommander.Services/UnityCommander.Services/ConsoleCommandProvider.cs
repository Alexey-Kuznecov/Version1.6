
using System.Collections.Generic;
using System.Linq;
using UnityCommander.CLI.Core;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.Services
{
    public class ConsoleCommandProvider : IConsoleCommandProvider
    {
        private readonly List<IConsoleCommand> _commands;

        public ConsoleCommandProvider(IEnumerable<IConsoleCommand> commands)
        {
            _commands = commands.ToList();
        }

        public IEnumerable<IConsoleCommand> GetAllCommands() => _commands;
    }
}
