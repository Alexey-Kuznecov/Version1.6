
using System.Collections.Generic;

namespace MultiColumns.DateTime
{
    using UnityCommander.Integration.Attributes;
    using UnityCommander.Integration.Options;

    /// <summary>
    /// The settings.
    /// </summary>
    public class DateTimeSettings : SettingsBase
    {
        private string dateTimeFormat;

        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeSettings"/> class.
        /// </summary>
        public DateTimeSettings()
        {
            this.DateTimeFormat = new string[]
            {
                "15.3.2008",
                "15/3/2008"
            };
        }

        /// <summary>
        /// Gets or sets the path with game the Witcher 3.
        /// </summary>
        [Option(
            "Формат даты и времени", 
            "В каком формате будет отображаться дата и время последнего изменения папки или файла",
            "Plugin")]
        public string[] DateTimeFormat { get; set; }

        public string GetDateTimeFormat() => this.dateTimeFormat;

        public void SetDateTimeFormat(object val)
        {
            if (val != null)
                this.dateTimeFormat = (string)val;
        }
    }
}
