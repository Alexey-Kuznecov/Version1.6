
namespace UnityCommander.Common.Styling.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media;

    using MaterialDesignThemes.Wpf;

    using UnityCommander.Integration.Enums;

    /// <summary>
    /// The icon converter.
    /// </summary>
    public class IconConverter : BaseConverter<IconConverter>
    {
        /// <summary>
        /// The convert back.
        /// </summary>
        /// <param name="value">
        /// The value produced by the original binding.
        /// </param>
        /// <param name="targetType">
        /// The type of the target binding property.
        /// </param>
        /// <param name="parameter">
        /// The converter parameter used.
        /// </param>
        /// <param name="culture">
        /// The language and regional settings used in the converter.
        /// </param>
        /// <returns>
        /// Converted value. If this method returns null, a valid NULL value is used.
        /// </returns>
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            PackIcon icon = new PackIcon();

            if (parameter is TargetPanel item)
            {
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
                }

                icon.Width = 25;
                return icon;
            }

            throw new Exception("");
        }

        /// <summary>
        /// The convert back.
        /// </summary>
        /// <param name="value">
        /// The value produced by the original binding.
        /// </param>
        /// <param name="targetType">
        /// The type of the target binding property.
        /// </param>
        /// <param name="parameter">
        /// The converter parameter used.
        /// </param>
        /// <param name="culture">
        /// The language and regional settings used in the converter.
        /// </param>
        /// <returns>
        /// Converted value. If this method returns null, a valid NULL value is used.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}