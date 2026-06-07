
using UnityCommander.Autocomplete.Definitions;
using UnityCommander.Autocomplete.Registary;

namespace UnityCommander.Autocomplete.Startup
{
    public sealed class CommandDescriptorInitializer
    {
        private readonly ICommandDescriptorRegistry _registry;

        public CommandDescriptorInitializer(
            ICommandDescriptorRegistry registry)
        {
            _registry = registry;
        }

        public void Initialize()
        {
            _registry.Register(
                new GitCommandDefinition());

            _registry.Register(
                new PluginCommandDefinition());

            //_registry.Register(
            //    new CopyDescriptor());
        }
    }
}
