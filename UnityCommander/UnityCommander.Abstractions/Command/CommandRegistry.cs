
namespace UnityCommander.Abstractions.Command
{
    public sealed class PluginCommandRegistry : IPluginCommandRegistry
    {
        private readonly Dictionary<string, ICommandDefinition> _commands = new();

        public void Register(ICommandDefinition definition)
        {
            _commands[definition.Id] = definition;
        }

        public bool TryGet(string id, out ICommandDefinition definition)
        {
            return _commands.TryGetValue(id, out definition);
        }

        public void Cleanup(string pluginId)
        {
            var keys = _commands
                .Where(x => x.Value.OwnerId == pluginId)
                .Select(x => x.Key)
                .ToList();

            foreach (var key in keys)
                _commands.Remove(key);
        }
    }
}
