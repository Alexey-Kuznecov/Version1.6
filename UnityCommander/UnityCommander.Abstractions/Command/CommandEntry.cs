
namespace UnityCommander.Abstractions.Command
{
    public sealed class CommandEntry
    {
        public string PluginId { get; }
        public ICommandDefinition Definition { get; }
    }
}
