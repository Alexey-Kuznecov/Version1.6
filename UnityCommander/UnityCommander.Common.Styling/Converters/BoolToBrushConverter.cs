using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace UnityCommander.Common.Styling.Converters
{
    /// <summary>
    /// Конвертер булевого значения в Brush.
    /// Используется, например, для подсветки выбранного элемента в UI.
    /// </summary>
    public class BoolToBrushConverter : IValueConverter
    {
        /// <summary>
        /// Кисть, которая будет применяться, если значение == true.
        /// </summary>
        public Brush SelectedBrush { get; set; } = Brushes.LightBlue;

        /// <summary>
        /// Кисть по умолчанию (если значение == false).
        /// </summary>
        public Brush DefaultBrush { get; set; } = Brushes.Transparent;

        /// <summary>
        /// Преобразует входное значение в Brush.
        /// Если value — это bool и он равен true — возвращается SelectedBrush.
        /// Иначе возвращается DefaultBrush.
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Проверяем, является ли входное значение bool и равно ли оно true
            if (value is bool b && b)
                return SelectedBrush;

            // В любых других случаях возвращаем кисть по умолчанию
            return DefaultBrush;
        }

        /// <summary>
        /// Обратное преобразование не требуется, поэтому возвращаем Binding.DoNothing,
        /// чтобы WPF знал, что обратная конвертация не выполняется.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => Binding.DoNothing;
    }
}
