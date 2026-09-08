
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace UnityCommander.Rendering.Converters
{
    public sealed class NullFallbackBrushConverter : IMultiValueConverter
    {
        public object Convert(
            object[] values,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            return values.FirstOrDefault(x => x is Brush)
                ?? DependencyProperty.UnsetValue;
        }

        public object[] ConvertBack(
            object value,
            Type[] targetTypes,
            object parameter,
            CultureInfo culture)
            => throw new NotSupportedException();
    }
}
