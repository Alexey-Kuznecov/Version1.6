
namespace UnityCommander.Common.Styling.Converters
{
    using System;
    using System.Globalization;

    /// <summary>
    /// The date time converter.
    /// </summary>
    public class DateTimeConverter : BaseConverter<DateTimeConverter>
    {
        /// <summary>
        /// The convert.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="targetType">
        /// The target type.
        /// </param>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        /// <param name="culture">
        /// The culture.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString();
        }
    }
}
