using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace UnityCommander.Modules.BottomPanel.Converter
{
    public sealed class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool visible))
                return Visibility.Collapsed;

            // Если параметр передан и равен "False" — инвертируем видимость
            if (parameter is string param && bool.TryParse(param, out bool invert) && invert == false)
                visible = !visible;

            return visible ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
