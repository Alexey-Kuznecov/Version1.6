
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using UnityCommander.Logging.Abstractions;

namespace UnityCommander.Modules.BottomPanel.Converter
{
    public class LogColorToBrushConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not LogColor color)
                return Brushes.White;

            return Application.Current.Resources[$"log.{color.Key}"];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Brushes.White;
        }
    }
}
