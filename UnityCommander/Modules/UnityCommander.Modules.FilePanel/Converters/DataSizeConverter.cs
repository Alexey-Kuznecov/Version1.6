using System;
using System.Globalization;
using System.Windows.Data;

namespace UnityCommander.Modules.FilePanel.Converters
{
    public class DataSizeConverter : IValueConverter
    {
        private static readonly string[] SizeSuffixes =
            { "B", "KB", "MB", "GB", "TB", "PB", "EB" };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return "0 B";

            double bytes;

            try
            {
                bytes = System.Convert.ToDouble(value);
            }
            catch
            {
                return "0 B";
            }

            if (bytes < 0)
                return "-" + Convert(-bytes, targetType, parameter, culture);

            int i = 0;
            while (bytes >= 1024 && i < SizeSuffixes.Length - 1)
            {
                bytes /= 1024;
                i++;
            }

            return $"{bytes:0.##} {SizeSuffixes[i]}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
