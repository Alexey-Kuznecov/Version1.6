
namespace UnityCommander.Common.Styling.Converters
{
    using MaterialDesignThemes.Wpf;
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media;
    using UnityCommander.Common.Enums;

    public class IconConverter : BaseConverter<IconConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            PackIcon icon = new PackIcon();

            if (parameter is DirectoryItemType item)
            {
                if (item == DirectoryItemType.Files) { 
                    icon.Kind = PackIconKind.File;
                    icon.Foreground = new SolidColorBrush(Color.FromRgb(64, 86, 141));
                }
                else { 
                    icon.Kind = PackIconKind.Folder;
                    icon.Foreground = new SolidColorBrush(Color.FromRgb(255, 165, 0));
                }

                icon.Width = 25;
                return icon;
            }

            throw new ArgumentException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}