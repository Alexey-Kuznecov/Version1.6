
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

        /// <summary>
        /// Основной Binding — значение, которое будет конвертироваться.
        /// </summary>
        public Binding Binding { get; set; }

        /// <summary>
        /// Режим привязки (OneWay, TwoWay, и т. д.)
        /// </summary>
        public BindingMode Mode { get; set; }

        /// <summary>
        /// Конвертер, который будет применён к значению.
        /// </summary>
        public IValueConverter Converter { get; set; }

        /// <summary>
        /// Дополнительный Binding, который будет передан как ConverterParameter.
        /// Позволяет динамически менять параметр конвертера.
        /// </summary>
        public Binding ConverterParameter { get; set; }

        #endregion

        #region Overridden Methods

        /// <summary>
        /// Формирует MultiBinding, который содержит:
        /// 1) основной Binding
        /// 2) Binding параметра (если задан)
        /// Использует адаптер для вызова обычного IValueConverter как IMultiValueConverter.
        /// </summary>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            // Создаём MultiBinding, т.к. нам нужно прокинуть 2 значения в конвертер.
            var multiBinding = new MultiBinding();

            // Настраиваем режим основного биндинга
            Binding.Mode = Mode;
            multiBinding.Bindings.Add(Binding);

            // Если есть биндинг для параметра — добавляем его как второй источник
            if (ConverterParameter != null)
            {
                ConverterParameter.Mode = BindingMode.OneWay;
                multiBinding.Bindings.Add(ConverterParameter);
            }

            // Оборачиваем обычный IValueConverter в адаптер IMultiValueConverter
            var adapter = new MultiValueConverterAdapter
            {
                Converter = Converter
            };

            multiBinding.Converter = adapter;

            // Возвращаем готовый MultiBinding в XAML
            return multiBinding.ProvideValue(serviceProvider);
        }

        #endregion

        /// <summary>
        /// Адаптер, который превращает обычный IValueConverter в IMultiValueConverter,
        /// позволяя прокидывать значение + параметр через MultiBinding.
        /// </summary>
        [ContentProperty(nameof(Converter))]
        private class MultiValueConverterAdapter : IMultiValueConverter
        {
            /// <summary>
            /// Хранит последний параметр, приходящий вторым значением.
            /// </summary>
            private object lastParameter;

            /// <summary>
            /// Обычный IValueConverter, который мы адаптируем.
            /// </summary>
            public IValueConverter Converter { get; set; }

            /// <summary>
            /// Вызывается при преобразовании вью → VM.
            /// </summary>
            public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
            {
                // Если конвертера нет — просто возвращаем основное значение
                // Нужно для дизайнеров VS, чтобы не падал XAML Designer.
                if (this.Converter == null)
                {
                    return values[0];
                }

                // Если подали параметр — запоминаем его
                if (values.Length > 1)
                {
                    this.lastParameter = values[1];
                }

                // Передаём основной value + динамический параметр
                return this.Converter.Convert(values[0], targetType, this.lastParameter, culture);
            }

            /// <summary>
            /// Обратное преобразование (если используется TwoWay).
            /// </summary>
            public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            {
                // Если нет конвертера — возвращаем значение как есть
                if (this.Converter == null)
                {
                    return new object[] { value };
                }

                // Вызываем ConvertBack обычного IValueConverter
                return new object[]
                {
                    this.Converter.ConvertBack(value, targetTypes[0], this.lastParameter, culture)
                };
            }
        }
    }
}
