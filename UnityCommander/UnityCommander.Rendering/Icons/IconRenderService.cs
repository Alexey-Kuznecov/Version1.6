
using System.Windows.Media;
using System.Windows.Shapes;
using UnityCommander.Abstractions.Icons;
using UnityCommander.Common.Diagnostic;

namespace UnityCommander.Rendering.Icons
{
    public class IconRenderService : IIconRenderService, IDiagnosticReporter
    {
        private readonly IIconResolver _resolver;
        private readonly IIconColorResolver _colorResolver;

        private readonly Dictionary<string, IconRenderResult?> _cache = new();

        private readonly Dictionary<string, Brush> _brushCache = new();

        public string Name =>  "icon.render.service";

        public DiagnosticCardinality Cardinality => DiagnosticCardinality.Single;

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
                Stroke = result.Stroke,
                StrokeThickness = result.StrokeWidth ?? 1,

                Width = result.Size,
                Height = result.Size,
                Stretch = Stretch.Uniform
            };
        }

        private IconRenderResult Render(RuntimeIcon definition)
        {
            var defaultColor =
                ResolveBrush(definition.Key?.ToLower());

            if (definition.Layers is { Count: > 0 })
                return RenderLayers(definition, defaultColor);

            return RenderLegacy(definition, defaultColor);
        }

        private IconRenderResult RenderLegacy(
                   RuntimeIcon definition,
                   Brush defaultColor)
        {
            var geometry = Geometry.Parse(definition.Data!);
            geometry.Freeze();

            return new IconRenderResult
            {
                Geometry = geometry,
                Brush = ResolveFill(definition, defaultColor),
                Stroke = ResolveStroke(definition, defaultColor)
            };
        }

        private IconRenderResult RenderLayers(
            RuntimeIcon definition,
            Brush defaultColor)
        {
            var geometryGroup = new GeometryGroup();

            foreach (var layer in definition.Layers!)
            {
                var geometry = Geometry.Parse(layer.Data);
                geometryGroup.Children.Add(geometry);
            }

            geometryGroup.Freeze();

            var firstLayer = definition.Layers[0];

            return new IconRenderResult
            {
                Geometry = geometryGroup,
                Brush = ResolveFill(firstLayer, defaultColor),
                Stroke = ResolveStroke(firstLayer, defaultColor),
                StrokeWidth = firstLayer.StrokeWidth
            };
        }

        private Brush? ResolveFill(
          RuntimeIconLayer layer,
          Brush defaultColor)
        {
            if (layer.Fill == null ||
                layer.Fill.Equals(
                    "none",
                    StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            return layer.Fill.Equals(
                "currentColor",
                StringComparison.OrdinalIgnoreCase)
                    ? defaultColor
                    : FromHex(layer.Fill);
        }

        private Brush? ResolveStroke(
            RuntimeIconLayer layer,
            Brush defaultColor)
        {
            if (layer.Stroke == null ||
                layer.Stroke.Equals(
                    "none",
                    StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            return layer.Stroke.Equals(
                "currentColor",
                StringComparison.OrdinalIgnoreCase)
                    ? defaultColor
                    : FromHex(layer.Stroke);
        }

        private Brush? ResolveFill(
            RuntimeIcon definition,
            Brush defaultColor)
        {
            if (definition.Color == null ||
                definition.Color.Equals(
                    "none",
                    StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            return definition.Color.Equals(
                "currentColor",
                StringComparison.OrdinalIgnoreCase)
                    ? defaultColor
                    : FromHex(definition.Color);
        }

        private Brush? ResolveStroke(
            RuntimeIcon definition,
            Brush defaultColor)
        {
            if (definition.Stroke == null ||
                definition.Stroke.Equals(
                    "none",
                    StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            return definition.Stroke.Equals(
                "currentColor",
                StringComparison.OrdinalIgnoreCase)
                    ? defaultColor
                    : FromHex(definition.Stroke);
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
                _ => Brushes.White
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

        public void Report(IDiagnosticWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
