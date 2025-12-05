
namespace UnityCommander.Modules.FilePanel.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Media;

    using MaterialDesignThemes.Wpf;
    using UnityCommander.Common;
    using UnityCommander.Common.Models.Directory;

    /// <summary>
    /// The icon converter.
    /// </summary>
    public class IconConverter : BaseConverter<IconConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            PackIcon icon = new PackIcon();

            if (parameter is TargetPanel item)
            {
                icon.Width = 25;
                
                switch (item)
                {
                    case TargetPanel.Folders:
                        icon.Kind = PackIconKind.Folder;
                        icon.Foreground = new SolidColorBrush(Color.FromRgb(255, 165, 0));
                        break;
                    case TargetPanel.Files:
                        icon.Kind = PackIconKind.File;
                        icon.Foreground = new SolidColorBrush(Color.FromRgb(64, 86, 141));
                        break;
                    case TargetPanel.LocalDisk:
                        icon.Kind = PackIconKind.Scanner;
                        icon.Foreground = new SolidColorBrush(Color.FromRgb(80, 119, 190));
                        break;
                }
                
                return icon;
            }

            throw new Exception("");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}