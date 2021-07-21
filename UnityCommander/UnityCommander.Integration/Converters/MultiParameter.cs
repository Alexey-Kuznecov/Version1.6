
namespace UnityCommander.Integration.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Markup;

    /// <summary>
    /// The bind-able parameter.
    /// </summary>
    [ContentProperty(nameof(Binding))]
    public class MultiParameter : MarkupExtension
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the binding.
        /// </summary>
        public Binding Binding { get; set; }

        /// <summary>
        /// Gets or sets the mode.
        /// </summary>
        public BindingMode Mode { get; set; }

        /// <summary>
        /// Gets or sets the converter.
        /// </summary>
        public IValueConverter Converter { get; set; }

        /// <summary>
        /// Gets or sets the converter parameter.
        /// </summary>
        public Binding ConverterParameter { get; set; }

        #endregion

        #region Overridden Methods

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
            var multiBinding = new MultiBinding();
            Binding.Mode = this.Mode;
            multiBinding.Bindings.Add(Binding);
            if (this.ConverterParameter != null)
            {
                this.ConverterParameter.Mode = BindingMode.OneWay;
                multiBinding.Bindings.Add(this.ConverterParameter);
            }

            var adapter = new MultiValueConverterAdapter
            {
                Converter = this.Converter
            };
            multiBinding.Converter = adapter;
            return multiBinding.ProvideValue(serviceProvider);
        }

        #endregion

        /// <summary>
        /// The multi value converter adapter.
        /// </summary>
        [ContentProperty(nameof(Converter))]
        private class MultiValueConverterAdapter : IMultiValueConverter
        {
            /// <summary>
            /// The last parameter.
            /// </summary>
            private object lastParameter;

            /// <summary>
            /// Gets or sets the converter.
            /// </summary>
            public IValueConverter Converter { get; set; }

            /// <summary>
            /// The convert.
            /// </summary>
            /// <param name="values">
            /// The values.
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
            public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
            {
                if (this.Converter == null)
                {
                    return values[0];
                } // Required for VS design-time

                if (values.Length > 1)
                {
                    this.lastParameter = values[1];
                }

                return this.Converter.Convert(values[0], targetType, this.lastParameter, culture);
            }

            /// <summary>
            /// The convert back.
            /// </summary>
            /// <param name="value">
            /// The value.
            /// </param>
            /// <param name="targetTypes">
            /// The target types.
            /// </param>
            /// <param name="parameter">
            /// The parameter.
            /// </param>
            /// <param name="culture">
            /// The culture.
            /// </param>
            /// <returns>
            /// The <see cref="object[]"/>.
            /// </returns>
            public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            {
                if (this.Converter == null)
                {
                    return new object[] { value };
                } // Required for VS design-time
            
                return new object[] { this.Converter.ConvertBack(value, targetTypes[0], this.lastParameter, culture) };
            }
        }
    }
}
