
namespace UnityCommander.Abstractions.Command
{
    public interface IPluginCommandRegistry
    {
        void Register(ICommandDefinition definition);

        bool TryGet(string id, out ICommandDefinition definition);

        void Cleanup(string pluginId);
    }
}
