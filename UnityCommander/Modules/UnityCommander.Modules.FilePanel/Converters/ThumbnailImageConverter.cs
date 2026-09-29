using System;
using System.Globalization;
using System.Windows.Data;
using UnityCommander.Abstractions.Thumbnail;
using UnityCommander.WPF.Images;

namespace UnityCommander.Modules.FilePanel.Converters
{
    public sealed class ThumbnailImageConverter : IValueConverter 
    { 
        public object? Convert(
            object value, 
            Type targetType, 
            object parameter, 
            CultureInfo culture)
        { 
            if (value is not ThumbnailResult thumbnail) 
                return null; 
            return ThumbnailImageCodec.Decode(thumbnail); 
        } 
        public object ConvertBack(
            object value, 
            Type targetType, 
            object parameter, 
            CultureInfo culture)
            => throw new NotSupportedException(); }
}
