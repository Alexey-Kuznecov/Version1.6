
using UnityCommander.Abstractions.Completion;

namespace UnityCommander.Testing.Infrastructure
{
    public sealed class CommandCatalog : ICommandCatalog
    {
        private readonly IReadOnlyList<ICommandDescriptor> _commands;

        public CommandCatalog(IEnumerable<ICommandDescriptor> commands)
        {
            _commands = commands.ToList();
        }

        public IEnumerable<ICommandDescriptor> GetAll() => _commands;
    }
}
