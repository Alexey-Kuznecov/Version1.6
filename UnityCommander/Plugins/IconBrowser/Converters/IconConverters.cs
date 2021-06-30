using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using AIconBrowser.Models;
using static System.Windows.Media.ColorConverter;

namespace AIconBrowser.Converters
{
    public class ColorConverterSolidColor : BaseConverter<ColorConverterSolidColor>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var combobox = value as ComboBoxItem;
            if (combobox != null)
                // ReSharper disable once PossibleNullReferenceException
                return new SolidColorBrush((Color)ConvertFromString(combobox.Content.ToString()));
            return null;
        }
    }
    public class ScaleConverter : BaseConverter<ScaleConverter>
    {
        public double Scale { get; set; } = 12;

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                double num = double.Parse(value.ToString());
                return (num * (Scale / 100));
            }
            return null;
        }
        public override object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }

    public class BrushConverter : BaseConverter<BrushConverter>
    {
        [SuppressMessage("ReSharper", "PossibleInvalidCastExceptionInForeachLoop")]
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            IconModel icon = value as IconModel;
            if (icon != null)
            {
                Brush brush = parameter?.ToString() == "rect" 
                    ? icon.BgroundColor 
                    : icon.FgroundColor;
                return brush;
            }
            return value;
        }
    }

    public class ContentConverter : BaseConverter<ContentConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Path pathdata = value as Path ?? (value as IconModel)?.Path;

            if (pathdata == null)
            {
                PathGeometry path = new PathGeometry();
                DrawingGroup draw = value is IconModel
                    ? ((IconModel)value).Brush.Drawing as DrawingGroup
                    : (value as DrawingBrush)?.Drawing as DrawingGroup;

                if (draw != null)
                    foreach (var drawing in draw.Children)
                    {
                        var item = (GeometryDrawing)drawing;
                        path.AddGeometry(item.Geometry);
                    }
                return path;
            }
            return pathdata.Data;
        }
    }
}
