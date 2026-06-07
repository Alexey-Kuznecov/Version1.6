
namespace UnityCommander.Common.Styling.Converters
{
    using System;
    using System.Globalization;
    using UnityCommander.Common;

    /// <summary>
    /// The date time converter.
    /// </summary>
    public class DateTimeConverter : BaseConverter<DateTimeConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString();
        }
    }
}
