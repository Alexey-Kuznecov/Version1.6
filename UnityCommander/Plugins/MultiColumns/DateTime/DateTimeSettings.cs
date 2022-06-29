
namespace MultiColumns.DateTime
{
    using UnityCommander.Integration.Attributes;
    using UnityCommander.Integration.Options;

    /// <summary>
    /// The settings.
    /// </summary>
    public class DateTimeSettings : SettingsBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeSettings"/> class.
        /// </summary>
        public DateTimeSettings()
        {
            this.GamePath = "C:\\Games\\The Witcher 3 Wild Hunt";
            this.GamePaths = new string[]
            {
                "Palit",
                "Tree",
                "List",
                "Cards"
            };
        }

        /// <summary>
        /// Gets or sets the path with game the Witcher 3.
        /// </summary>
        [Option("Попробуйте новую кроссплатформенную оболочку PowerShell (https://aka.ms/pscore6)")]
        public string GamePath { get; set; }

        /// <summary>
        /// Gets or sets the path with game the Witcher 3.
        /// </summary>
        [Option("Controls whether the editor should run in a mode where it is optimized for screen readers. Setting to on will disable word wrapping.")]
        public string[] GamePaths { get; set; }
    }
}
