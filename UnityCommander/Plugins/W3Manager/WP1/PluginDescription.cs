
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
        public string DisplayName { get; set; } = "W3 Manager Mod";

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; } = "Plug-in for your W3 Manager Mod.";
    }
}
