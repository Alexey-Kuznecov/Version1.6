
namespace UnityCommander.Common.Styling.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Markup;

    /// <summary>
    /// The base converter.
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
        /// Must be implemented in inheritor.
        /// </summary>
        /// <param name="value"> The <c>value</c>. </param>
        /// <param name="targetType"> The target type. </param>
        /// <param name="parameter"> The <c>parameter</c>. </param>
        /// <param name="culture"> The <c>culture</c>. </param>
        /// <returns> The <see cref="object"/>. </returns>
        public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);

        /// <summary>
        /// Override if needed.
        /// </summary>
        /// <param name="value"> The value. </param>
        /// <param name="targetType"> The target type. </param>
        /// <param name="parameter"> The parameter. </param>
        /// <param name="culture"> The culture. </param>
        /// <returns> The <see cref="object"/>. </returns>
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
