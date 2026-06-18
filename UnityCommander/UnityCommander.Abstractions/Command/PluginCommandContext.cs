
namespace UnityCommander.Abstractions.Commands
{
    public class PluginCommandContext
    {
        public IServiceProvider Services { get; init; }
        
        public IRuntimeServices Runtime { get; init; }

        public string PluginId { get; init; }

        public object? Parameter { get; init; }
    }
}
