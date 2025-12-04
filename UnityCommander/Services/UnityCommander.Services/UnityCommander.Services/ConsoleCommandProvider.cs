
using System.Collections.Generic;
using System.Linq;
using UnityCommander.Common.Commands;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.Services
{
    public class ConsoleCommandProvider : IConsoleCommandProvider
    {
        private readonly List<IConsoleCommandBase> _commands;

        public ConsoleCommandProvider(IEnumerable<IConsoleCommandBase> commands)
        {
            _commands = commands.ToList();
        }

        public IEnumerable<IConsoleCommandBase> GetAllCommands() => _commands;
    }
}
