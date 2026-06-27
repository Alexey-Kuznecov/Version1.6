
using System.Windows.Media;
using System.Windows.Shapes;
using UnityCommander.Abstractions.Icons;

namespace UnityCommander.Rendering.Icons
{
    public class IconRenderService : IIconRenderService
    {
        private readonly IIconResolver _resolver;
        private readonly IIconColorResolver _colorResolver;

        private readonly Dictionary<string, IconRenderResult?> _cache = new();

        private readonly Dictionary<string, Brush> _brushCache = new();

        public IconRenderService(IIconResolver resolver, IIconColorResolver colorResolver)
        {
            _resolver = resolver;
            _colorResolver = colorResolver;
        }

        public bool TryGet(string key, out IconRenderResult result)
        {
            if (_cache.TryGetValue(key, out result))
                return true;

            if (!_resolver.TryResolve(key, out var definition))
                return false;

            result = Render(definition);

            _cache[key] = result;

            return true;
        }

        public Path GetPath(string key)
        {
            if (!TryGet(key, out var result))
                return new Path();

            return CreatePath(result);
        }

        public Path CreatePath(IconRenderResult result)
        {
            return new Path
            {
                Data = result.Geometry,
                Fill = result.Brush,
                Width = result.Size,
                Height = result.Size,
                Stretch = Stretch.Uniform
            };
        }

        private IconRenderResult Render(RuntimeIcon definition)
        {
            var geometry = Geometry.Parse(definition.Data);

            geometry.Freeze();

            return new IconRenderResult
            {
                Geometry = geometry,
                Brush = ResolveBrush(definition?.Key?.ToLower())
            };

        }

        private Brush ResolveBrush(string? key)
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
                _ => Brushes.Black
            };
        }

        private Brush FromHex(string hex)
        {
            if (_brushCache.TryGetValue(hex, out var brush))
                return brush;

            var parsed = new SolidColorBrush(
                (Color)ColorConverter.ConvertFromString(hex));

            parsed.Freeze(); // важно для WPF performance
            _brushCache[hex] = parsed;

            return parsed;
        }
    }
}
