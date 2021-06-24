
namespace UnityCommander.Common.Styling.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Markup;

    /// <summary>
    /// The base converter 
    /// </summary>
    /// <typeparam name="T"> General Type. </typeparam>
    public abstract class BaseConverter<T> : MarkupExtension, IValueConverter
        where T : class, new()
    {
        /// <summary>
        /// The _converter.
        /// </summary>
        private static T converter = null;

        /// <summary>
        /// The data binding handler calls this method when it propagates 
        /// a value from the binding source to the binding target.
        /// Must be implemented in inheritor.
        /// </summary>
        /// <param name="value"> The value produced by the original binding. </param>
        /// <param name="targetType"> The type of the target binding property. </param>
        /// <param name="parameter"> The converter parameter used. </param>
        /// <param name="culture"> The language and regional settings used in the converter. </param>
        /// <returns> Converted value. If this method returns null, a valid NULL value is used. </returns>
        public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);

        /// <summary>
        /// Override if needed.
        /// </summary>
        /// <param name="value"> The value produced by the original binding. </param>
        /// <param name="targetType"> The type of the target binding property. </param>
        /// <param name="parameter"> The converter parameter used. </param>
        /// <param name="culture"> The language and regional settings used in the converter. </param>
        /// <returns> Converted value. If this method returns null, a valid NULL value is used. </returns>
        public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #region MarkupExtension members

        /// <summary>
        /// The provide value.
        /// </summary>
        /// <param name="serviceProvider">
        /// The service provider.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return converter ?? (converter = new T());
        }

        #endregion
    }
}
