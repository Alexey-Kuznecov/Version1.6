
namespace W3Manager.WP1
{
    using UnityCommander.Integration.Attributes;
    using UnityCommander.Integration.Options;

    /// <summary>
    /// The settings.
    /// </summary>
    public class ModSettings : SettingsBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModSettings"/> class.
        /// </summary>
        public ModSettings()
        {
            this.GamePath = "C:\\Games\\The Witcher 3 Wild Hunt";
        }

        /// <summary>
        /// Gets or sets the path with game the Witcher 3.
        /// </summary>
        [Option("Укажите путь к игре: ")]
        public string GamePath { get; set; }

        /// <summary>
        /// Gets or sets the path with game the Witcher 3.
        /// </summary>
        [Option("Укажите путь к игре: ")]
        public string[] GamePaths { get; set; }
    }
}
