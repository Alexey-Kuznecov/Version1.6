
namespace W3Manager.WP1
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
                                                              "Set Game Column",
                                                              "Set Game Cell"
                                                          };
    }
}
