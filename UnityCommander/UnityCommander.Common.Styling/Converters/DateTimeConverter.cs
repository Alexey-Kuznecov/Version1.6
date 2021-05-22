
namespace UnityCommander.Common.Styling.Converters
{
    using System;
    using System.Globalization;

    public class DateTimeConverter : BaseConverter<DateTimeConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString();
        }
    }
}
