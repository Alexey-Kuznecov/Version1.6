
namespace W3Manager.WP1
{
    using UnityCommander.Integration.Contracts;
    using UnityCommander.Integration.Options;

    /// <summary>
    /// The game category column.
    /// </summary>
    public class DateTimeColumn : IOptionBuilder, IPluginDescriptor
    {
        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        public string DisplayName { get; set; } = "W3 Manager Mod";

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; } = "Plug-in for your W3 Manager Mod.";

        /// <summary>
        /// The option build.
        /// </summary>
        /// <param name="optionBuilder">
        /// The option builder.
        /// </param>
        public void OptionBuild(OptionBuilder optionBuilder)
        {
            optionBuilder.Add();
        }
    }
}
