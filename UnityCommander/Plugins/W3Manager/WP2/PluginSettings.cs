
namespace W3Manager.WP2
{
    using System.Collections.Generic;

    /// <summary>
    /// The plugin settings.
    /// </summary>
    public class PluginSettings
    {
        /// <summary>
        /// Gets or sets the display as.
        /// </summary>
        public List<string> DisplayAs { get; set; } = new List<string>
                                                          {
                                                              "Set Mod Column",
                                                              "Set Mod Cell"
                                                          };
    }
}
