
namespace MultiColumns.DateTime
{
    using UnityCommander.Integration.Attributes;
    using UnityCommander.Integration.Options;

    public class DateTimeSettings : SettingsBase
    {
        private string dateTimeFormat;

        public DateTimeSettings()
        {
            this.DateTimeFormat = new string[]
            {
                "15.3.2008",
                "15/3/2008"
            };
        }

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
