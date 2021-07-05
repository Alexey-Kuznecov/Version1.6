
namespace MultiColumns.Image
{
    using System.Diagnostics;
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
            this.ImageSize = "200x200";
        }

        /// <summary>
        /// Gets or sets the date format.
        /// </summary>
        [AttachHandler(OptionRender.TextField, typeof(PluginSettings), nameof(ImageSizeHandler), typeof(OptionHandler))]
        [OptionDescription("Change date and time output format:")]
        public string ImageSize { get; set; }

        /// <summary>
        /// The image size handler.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <returns>
        /// The string
        /// </returns>
        public string ImageSizeHandler(string path)
        {
            Debug.WriteLine("This is ImagesColumns plugin!");
            Debug.WriteLine(path);

            return string.Empty;
        }
    }
}
