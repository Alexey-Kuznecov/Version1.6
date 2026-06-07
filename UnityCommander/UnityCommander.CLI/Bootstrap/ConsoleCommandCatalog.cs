
using System.Reflection;
using UnityCommander.CLI.Integration;

namespace UnityCommander.CLI.Bootstrap
{
    public sealed class ConsoleCommandCatalog
    {
        private readonly Dictionary<string, Type> _commands = new();

        public void Register(Type commandType)
        {
            var attribute =
                commandType.GetCustomAttribute<ConsoleCommandAttribute>();

            if (attribute == null)
                return;

            _commands[attribute.Name] = commandType;
        }

        public Type? Get(string name)
        {
            return _commands.TryGetValue(name, out var type)
                ? type
                : null;
        }

        public IReadOnlyDictionary<string, Type> Commands => _commands;
    }
}
