
using System.Windows.Media;
using UnityCommander.Abstractions.Icons;
using UnityCommander.Rendering.Converters;
using UnityCommander.Rendering.Icons.Services;

namespace UnityCommander.Rendering.Icons.Strategies
{
    public sealed class LayeredIconRenderStrategy : IIconRenderStrategy
    {
        public IconType Type => IconType.Layered;

        private readonly IIconRenderNormalizer _normalizer;

        public LayeredIconRenderStrategy(IIconRenderNormalizer normalizer)
        {
            _normalizer = normalizer;
        }

        public IconRenderResult Render(
            RuntimeIcon icon,
            Brush defaultBrush)
        {
            var layers = icon.Layers
                .Select((layer, index) => new IconRenderLayer
                {
                    Geometry = ParseGeometry(layer.Data),
                    Fill = ResolveBrush(layer.Fill, defaultBrush),
                    Stroke = ResolveBrush(layer.Stroke, defaultBrush),
                    StrokeWidth = layer.StrokeWidth,
                    StrokeLineCap = ParseLineCap(layer.StrokeLineCap),
                    StrokeLineJoin = ParseLineJoin(layer.StrokeLineJoin),
                    Order = index
                })
                .ToList();

            return _normalizer.Normalize(layers);
        }

        private static Geometry ParseGeometry(string data)
        {
            var geometry = Geometry.Parse(data);
            geometry.Freeze();
            return geometry;
        }

        private static Brush? ResolveBrush(
            string? value,
            Brush defaultBrush)
        {
            if (string.IsNullOrWhiteSpace(value) ||
                value.Equals("none", StringComparison.OrdinalIgnoreCase))
                return null;

            if (value.Equals(
                "currentColor",
                StringComparison.OrdinalIgnoreCase))
                return defaultBrush;

            return BrushColorHelper.StringFormatToSolidColor(value);
        }

        private static PenLineCap? ParseLineCap(string? value)
        {
            return value?.ToLowerInvariant() switch
            {
                "flat" => PenLineCap.Flat,
                "square" => PenLineCap.Square,
                "round" => PenLineCap.Round,
                _ => null
            };
        }

        private static PenLineJoin? ParseLineJoin(string? value)
        {
            return value?.ToLowerInvariant() switch
            {
                "miter" => PenLineJoin.Miter,
                "bevel" => PenLineJoin.Bevel,
                "round" => PenLineJoin.Round,
                _ => null
            };
        }
    }
}
