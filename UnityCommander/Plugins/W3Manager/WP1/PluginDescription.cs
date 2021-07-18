
using UnityCommander.Integration.Contracts;

namespace W3Manager.WP1
{
    class PluginDescription : IPluginDescriptor
    {
        public string DisplayName { get; set; } = "Game columns";
        public string Description { get; set; } = "Columns for your game.";
    }
}
