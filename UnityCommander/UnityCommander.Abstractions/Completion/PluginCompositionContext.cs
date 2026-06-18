
using UnityCommander.Abstractions.Command;
using UnityCommander.Abstractions.Plugins;

namespace UnityCommander.Abstractions.Plugin
{
    public class PluginCompositionContext
    {
        public string? PluginId { get; set; }

        public IServiceProvider? Services { get; set; }

        public IMessageBus? Bus { get; set; }

        public IPluginCommandRegistry? Registry { get; set; }
    }
}
