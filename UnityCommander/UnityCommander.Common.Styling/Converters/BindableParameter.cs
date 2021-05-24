using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using System.Windows.Markup;

namespace UnityCommander.Common.Styling.Converters
{
    [ContentProperty(nameof(Binding))]
    public class BindableParameter : MarkupExtension
    {
        #region Public Properties

        public Binding Binding { get; set; }
        public BindingMode Mode { get; set; }
        public IValueConverter Converter { get; set; }
        public Binding ConverterParameter { get; set; }

        #endregion

        public BindableParameter()
        { }

        public BindableParameter(string path)
        {
            Binding = new Binding(path);
        }

        public BindableParameter(Binding binding)
        {
            Binding = binding;
        }

        #region Overridden Methods

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
            public IValueConverter Converter { get; set; }

            private object lastParameter;

            public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
            {
                if (Converter == null) return values[0]; // Required for VS design-time
                if (values.Length > 1) lastParameter = values[1];
                return Converter.Convert(values[0], targetType, lastParameter, culture);
            }

            public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            {
                if (Converter == null) return new object[] { value }; // Required for VS design-time

                return new object[] { Converter.ConvertBack(value, targetTypes[0], lastParameter, culture) };
            }
        }
    }
}
