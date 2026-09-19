
namespace UnityCommander.Modules.FilePanel.Behaviors
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;
    using System.Windows.Data;
    using System.Windows.Markup;
    using UnityCommander.Abstractions.Panels;

    /// <summary>
    /// The bind-able parameter.
    /// </summary>
    [ContentProperty(nameof(Binding))]
    public class BindableParameter : MarkupExtension
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
            Binding.Mode = Mode;
            multiBinding.Bindings.Add(Binding);
            if (ConverterParameter != null)
            {
                ConverterParameter.Mode = BindingMode.OneWay;
                multiBinding.Bindings.Add(ConverterParameter);
            }

            var adapter = new MultiValueConverterAdapter
            {
                Converter = Converter
            };
            multiBinding.Converter = adapter;
            return multiBinding.ProvideValue(serviceProvider);
        }

        #endregion

        [ContentProperty(nameof(Converter))]
        private class MultiValueConverterAdapter : IMultiValueConverter
        {
            private object lastParameter;

            public IValueConverter Converter { get; set; }

            public object Convert(
                object[] values,
                Type targetType,
                object parameter,
                CultureInfo culture)
            {
                if (Converter == null)
                    return values[0];

                if (values.Length > 1)
                    lastParameter = values[1];

                var cell = parameter as ICellModel;

                var index = cell.OutputOrder;

                cell.OutputOrder++;

                return Converter.Convert(
                    values[0],
                    targetType,
                    index,
                    culture);
            }
        

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
