using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace UnityCommander.Modules.FilePanel.Converters
{
    public class BoolToBrushConverter : IValueConverter
    {
        public Brush SelectedBrush { get; set; } = Brushes.LightBlue;
        public Brush DefaultBrush { get; set; } = Brushes.Transparent;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b && b) return SelectedBrush;
            return DefaultBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;
    }
}
