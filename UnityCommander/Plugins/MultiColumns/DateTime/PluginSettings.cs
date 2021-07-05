
namespace MultiColumns.DateTime
{
    using System.Collections.Generic;
    using UnityCommander.Integration.Attributes;
    using UnityCommander.Integration.Contracts;
    using UnityCommander.Integration.Options;

    /// <summary>
    /// The plugin settings.
    /// </summary>
    public class PluginSettings : IPluginConfigure
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PluginSettings"/> class.
        /// </summary>
        public PluginSettings()
        {
            this.DateFormat = new List<DropBoxModel>
            {
                new DropBoxModel { Content = "MM/DD/YY" },
                new DropBoxModel { Content = "DD/MM/YY" },
                new DropBoxModel { Content = "YY/MM/DD" }
            };
        }

        /// <summary>
        /// Gets or sets the date format.
        /// </summary>
        [OptionDescription("Change date and time output format:")]
        [AttachHandler(OptionRender.DropBox, typeof(PluginOptionHandler), nameof(PluginOptionHandler.DateFormatHandler), typeof(OptionHandler))]
        public List<DropBoxModel> DateFormat { get; set; }
    }
}
