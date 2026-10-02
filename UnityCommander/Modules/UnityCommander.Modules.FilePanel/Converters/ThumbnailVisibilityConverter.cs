
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using UnityCommander.Abstractions.Thumbnail;

namespace UnityCommander.Modules.FilePanel.Converters
{
    public sealed class ThumbnailVisibilityConverter : IValueConverter
    {
        public object Convert(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            var hasThumbnail = value is ThumbnailResult;

            var inverse =
                parameter?.ToString() == "Inverse";

            if (inverse)
                hasThumbnail = !hasThumbnail;

            return hasThumbnail
                ? Visibility.Visible
                : Visibility.Collapsed;
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
