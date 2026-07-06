
namespace UnityCommander.Modules.FilePanel.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Markup;

    /// <summary>
    /// Позволяет передавать параметры в конвертер через биндинги.
    /// Это расширение разметки, которое создаёт MultiBinding и адаптирует обычный IValueConverter
    /// к использованию с несколькими значениями (основной Binding + Binding параметра).
    /// </summary>
    [ContentProperty(nameof(Binding))]
    public class BindableParameter : MarkupExtension
    {
        #region Public Properties

        public Binding Binding { get; set; }

        public BindingMode Mode { get; set; }

        public IValueConverter Converter { get; set; }

        public Binding ConverterParameter { get; set; }

        #endregion

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
            private object lastParameter;

            public IValueConverter Converter { get; set; }

            public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
            {
                if (this.Converter == null)
                {
                    return values[0];
                }

                if (values.Length > 1)
                {
                    this.lastParameter = values[1];
                }

                return this.Converter.Convert(values[0], targetType, this.lastParameter, culture);
            }

            public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            {
                if (this.Converter == null)
                {
                    return new object[] { value };
                }

                return new object[]
                {
                    this.Converter.ConvertBack(value, targetTypes[0], this.lastParameter, culture)
                };
            }
        }
    }
}
