
namespace MultiColumns.Images
{
    using UnityCommander.Integration.Contracts;
    using UnityCommander.Integration.Enums;

    /// <summary>
    /// The image model.
    /// </summary>
    internal class ImageModel
    {
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        [ValueHandler(OptionRender.TextBlock, TargetPanel.Files,
            typeof(PluginOptionHandler),
            nameof(PluginOptionHandler.GetDpiValue), 
            typeof(InsertValueUsePath))]
        public string Dpi { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        [ValueHandler(OptionRender.TextBlock, TargetPanel.Files,
            typeof(PluginOptionHandler),
            nameof(PluginOptionHandler.GetSizeValue), 
            typeof(InsertValueUsePath))]
        public string Sized { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        [ValueHandler(OptionRender.TextBlock, TargetPanel.Files,
            typeof(PluginOptionHandler),
            nameof(PluginOptionHandler.GetColorValue), 
            typeof(InsertValueUsePath))]
        public string Colors { get; set; }
    }
}
