
using IconBrowser.Models;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace IconBrowser.Converters
{
    public sealed class IconViewTemplateConverter : IValueConverter
    {
        public object Convert(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            if (value is not IconViewMode mode)
                return null!;

            var resources = Application.Current.Resources;

            return mode switch
            {
                IconViewMode.List => (ControlTemplate)Application.Current.TryFindResource("ListTemplate"),
                IconViewMode.Tile => (ControlTemplate)Application.Current.TryFindResource("TileTemplate"),
                IconViewMode.Card => (ControlTemplate)Application.Current.TryFindResource("CardTemplate"),
                _ => (ControlTemplate)Application.Current.TryFindResource("TileTemplate")  
            };
        }

        public object ConvertBack(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
