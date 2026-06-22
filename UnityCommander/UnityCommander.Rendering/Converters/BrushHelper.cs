
using System.Windows.Media;

namespace UnityCommander.Rendering.Converters
{
    public static class BrushColorHelper
    {
        public static SolidColorBrush StringFormatToSolidColor(this string value)
        {
            var color = (Color)ColorConverter.ConvertFromString(value);
            return new SolidColorBrush(color);
        }
    }
}
