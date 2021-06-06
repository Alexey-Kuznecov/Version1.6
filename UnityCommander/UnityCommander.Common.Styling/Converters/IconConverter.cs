
namespace UnityCommander.Common.Styling.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media;

    using MaterialDesignThemes.Wpf;

    using UnityCommander.Common.Enums;

    /// <summary>
    /// The icon converter.
    /// </summary>
    public class IconConverter : BaseConverter<IconConverter>
    {
        /// <summary>
        /// The convert.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="targetType">
        /// The target type.
        /// </param>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        /// <param name="culture">
        /// The culture.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            PackIcon icon = new PackIcon();

            if (parameter is DirectoryItemType item)
            {
                if (item == DirectoryItemType.Files) 
                { 
                    icon.Kind = PackIconKind.File;
                    icon.Foreground = new SolidColorBrush(Color.FromRgb(64, 86, 141));
                }
                else 
                { 
                    icon.Kind = PackIconKind.Folder;
                    icon.Foreground = new SolidColorBrush(Color.FromRgb(255, 165, 0));
                }

                icon.Width = 25;
                return icon;
            }

            throw new ArgumentException();
        }

        /// <summary>
        /// The convert back.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="targetType">
        /// The target type.
        /// </param>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        /// <param name="culture">
        /// The culture.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}