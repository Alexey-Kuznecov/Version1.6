
namespace W3Manager.WP1
{
    using UnityCommander.Integration.Contracts;

    /// <summary>
    /// The plugin description.
    /// </summary>
    public class PluginDescription : IPluginDescriptor
    {
        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        public string DisplayName { get; set; } = "Game columns";

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; } = "Columns for your game.";
    }
}
