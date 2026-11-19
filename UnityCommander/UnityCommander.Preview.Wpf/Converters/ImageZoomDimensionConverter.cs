using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace UnityCommander.Preview.Wpf.Converters;

public sealed class ImageZoomDimensionConverter : IMultiValueConverter
{
    public object Convert(
        object[] values,
        Type targetType,
        object parameter,
        CultureInfo culture)
    {
        if (values.Length < 5 ||
            !TryGetNumber(values[0], out var pixelWidth) ||
            !TryGetNumber(values[1], out var pixelHeight) ||
            pixelWidth <= 0 ||
            pixelHeight <= 0)
        {
            return 0d;
        }

        var zoomText = values[4] as string ?? "Fit";

        double scale;

        if (zoomText.Equals(
            "Fit",
            StringComparison.OrdinalIgnoreCase))
        {
            if (!TryGetNumber(values[2], out var viewportWidth) ||
                !TryGetNumber(values[3], out var viewportHeight) ||
                viewportWidth <= 0 ||
                viewportHeight <= 0)
            {
                scale = 1;
            }
            else
            {
                scale = Math.Min(
                    (viewportWidth - 2) / pixelWidth,
                    (viewportHeight - 2) / pixelHeight);

                scale = Math.Max(0.01, scale);
            }
        }
        else
        {
            var number = zoomText.TrimEnd('%');

            if (!double.TryParse(
                number,
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out var zoom))
            {
                scale = 1;
            }
            else
            {
                scale = zoom / 100d;
            }
        }

        return string.Equals(
            parameter as string,
            "Height",
            StringComparison.OrdinalIgnoreCase)
                ? pixelHeight * scale
                : pixelWidth * scale;
    }

    public object[] ConvertBack(
        object value,
        Type[] targetTypes,
        object parameter,
        CultureInfo culture)
    {
        throw new NotSupportedException();
    }

    private static bool TryGetNumber(object value, out double number)
    {
        number = 0;

        if (value == DependencyProperty.UnsetValue)
            return false;

        if (value is double doubleValue)
        {
            number = doubleValue;
            return true;
        }

        if (value is int intValue)
        {
            number = intValue;
            return true;
        }

        return false;
    }
}