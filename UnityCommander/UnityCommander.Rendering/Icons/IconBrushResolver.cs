
using System.Windows.Media;

namespace UnityCommander.Rendering.Icons
{
    public sealed class IconBrushResolver : IIconBrushResolver
    {
        private readonly Dictionary<string, Brush> _cache = new();

        public Brush Resolve(string? key)
        {
            return key switch
            {
                "core.folder" => Brushes.Orange,
                "core.file" => Brushes.SteelBlue,
                "core.drive" => Brushes.Gray,

                "core.foldertree" => FromHex("#FF1368"),
                "core.column" => FromHex("#FF1368"),
                "core.plugins" => FromHex("#FF1368"),
                "core.commnet" => FromHex("#FF1368"),
                "core.tag" => FromHex("#FF1368"),
                "core.git" => FromHex("#FF1368"),
                "core.sack" => FromHex("#FF1368"),

                _ => Brushes.White
            };
        }

        private Brush FromHex(string hex)
        {
            if (_cache.TryGetValue(hex, out var brush))
                return brush;

            var parsed = new SolidColorBrush(
                (Color)ColorConverter.ConvertFromString(hex));

            parsed.Freeze();

            _cache[hex] = parsed;

            return parsed;
        }
    }
}
