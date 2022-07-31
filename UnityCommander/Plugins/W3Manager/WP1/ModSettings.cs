
namespace W3Manager.WP1
{
    using UnityCommander.Integration.Attributes;
    using UnityCommander.Integration.Options;

    /// <summary>
    /// The settings.
    /// </summary>
    public class ModSettings : SettingsBase
    {
        private string showModStatus;

        /// <summary>
        /// Initializes a new instance of the <see cref="ModSettings"/> class.
        /// </summary>
        public ModSettings()
        {
            this.GamePath = "C:\\Games\\The Witcher 3 Wild Hunt";
            //this.GamePaths = new string[]
            //{
            //    "Palit",
            //    "Tree",
            //    "List",
            //    "Cards"
            //};

            //this.GamePaths2 = new string[]
            //{
            //    "Auto",
            //    "On",
            //    "Off"
            //};

            this.ShowModStatus = new string[]
            {
                "On",
                "Off"
            };
        }

        /// <summary>
        /// Gets or sets the path with game the Witcher 3.
        /// </summary>
        [Option("Попробуйте новую кроссплатформенную оболочку PowerShell (https://aka.ms/pscore6)")]
        public string GamePath { get; set; }

        public void SetGamePath(object val)
        {
            this.GamePath = (string)val;
        }

        /// <summary>
        /// Gets or sets the path with game the Witcher 3.
        /// </summary>
        [Option("Controls whether the editor should run in a mode where it is optimized for screen readers. Setting to on will disable word wrapping.")]
        public string[] ShowModStatus { get; set; }

        public void SetShowModStatus(object val)
        {
            if (val != null)
                this.showModStatus = (string)val;
        }

        public string GetShowModStatus()
        {
            return this.showModStatus;
        }

        /// <summary>
        /// Gets or sets the path with game the Witcher 3.
        /// </summary>
        //[Option("Controls whether the search string in the Find Widget is seeded from the editor selection.")]
        //public string[] GamePaths2 { get; set; }
    }
}
