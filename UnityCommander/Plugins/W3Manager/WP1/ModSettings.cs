
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

        private string showModCategory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ModSettings"/> class.
        /// </summary>
        public ModSettings()
        {
            this.GamePath = "C:\\Games\\The Witcher 3 Wild Hunt";
            this.ShowModStatus = new string[]
            {
                "On",
                "Off"
            };
            this.ShowModCategory = new string[]
            {
                "On",
                "Off"
            };
        }

        /// <summary>
        /// Gets or sets the path with game the Witcher 3.
        /// </summary>
        [Option(
            "Путь к игре:",
            "Необходимо указать путь к игре для правильной работы плагина (https://aka.ms/pscore6).",
            "Plugin")]
        public string GamePath { get; set; }

        /// <summary>
        /// Gets or sets the path with game the Witcher 3.
        /// </summary>
        [Option(
            "Показывать колонку состояние мода",
            "Показывать колонку которая показывает установлен ли мод.", 
            "Plugin")]
        public string[] ShowModStatus { get; set; }

        /// <summary>
        /// Gets or sets the path with game the Witcher 3.
        /// </summary>
        [Option(
            "Показывать колонку категории мода",
            "Например моды могут быть графическими, геймлейными итд.",           
            "Plugin")]
        public string[] ShowModCategory { get; set; }

        public void SetGamePath(object val) => this.GamePath = (string)val;

        public void SetShowModCategory(object val)
        {
            if (val != null)
                this.showModCategory = (string)val;
        }

        public void SetShowModStatus(object val)
        {
            if (val != null)
                this.showModStatus = (string)val;
        }

        public string GetShowModStatus() => this.showModStatus;

        public string GetShowModCategory() => this.showModCategory;
    }
}
