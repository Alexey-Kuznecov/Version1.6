
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace UnityCommander.Modules.FilePanel.Converters
{
    public class RandomForegroundConverter : IValueConverter
    {
        private static readonly Brush[] Brushes =
        {
            System.Windows.Media.Brushes.Red,
            System.Windows.Media.Brushes.Green,
            System.Windows.Media.Brushes.Blue,
            System.Windows.Media.Brushes.Orange,
            System.Windows.Media.Brushes.BlueViolet,
            System.Windows.Media.Brushes.Gold,
            System.Windows.Media.Brushes.Cyan,
        };

        private static readonly Random Random = new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Brushes[Random.Next(Brushes.Length)];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => Binding.DoNothing;
    }
}
