using System;
using System.Collections.Generic;
using System.Text;
using MultiColumns.DateTime;
using UnityCommander.Integration.Attributes;
using UnityCommander.Integration.Options;

namespace MultiColumns.Sized
{
    public class SizeSettings : SettingsBase
    {
        private string sizedUnit;

        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeSettings"/> class.
        /// </summary>
        public SizeSettings()
        {
            this.sizedUnit = "Auto";

            this.SizedUnit = new string[]
            {
                "Auto",
                "In bytes",
                "In kbyte",
                "In mbyte",
                "In gbyte"
            };
        }

        /// <summary>
        /// Gets or sets the path with game the Witcher 3.
        /// </summary>
        [Option(
            "Формат даты и времени",
            "В каком формате будет отображаться дата и время последнего изменения папки или файла",
            "Plugin")]
        public string[] SizedUnit { get; set; }

        public string GetSizedUnit() => this.sizedUnit;

        public void SetSizedUnit(object val)
        {
            if (val != null)
                this.sizedUnit = (string)val;
        }
    }
}
