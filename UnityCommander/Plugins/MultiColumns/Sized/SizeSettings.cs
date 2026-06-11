
using UnityCommander.Integration.Attributes;
using UnityCommander.Integration.Options;

namespace MultiColumns.Sized
{
    public class SizeSettings : SettingsBase
    {
        private string sizedUnit;

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
