
using System;
using System.Globalization;
using System.Windows.Data;

namespace UnityCommander.Modules.FilePanel.Converters
{
    public class MultiParameterConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            Tuple<string, string> tuple = new Tuple<string, string>(
                (string)values[0], (string)values[1]);
            return (object)tuple;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
