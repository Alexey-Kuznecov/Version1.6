
using System.Windows.Media;
using UnityCommander.Abstractions.Icons;
using UnityCommander.Rendering.Converters;
using UnityCommander.Rendering.Icons.Services;

namespace UnityCommander.Rendering.Icons.Strategies
{
    public sealed class StrokeIconRenderStrategy : IIconRenderStrategy
    {
        public IconType Type => IconType.Stroke;

        private readonly IIconRenderNormalizer _normalizer;

        public StrokeIconRenderStrategy(IIconRenderNormalizer normalizer)
        {
            _normalizer = normalizer;
        }

        public IconRenderResult Render(
            RuntimeIcon icon,
            Brush defaultBrush)
        {
            var geometry = Geometry.Parse(icon.Data!);
            geometry.Freeze();

            var result = new IconRenderResult
            {
                Layers =
                [
                    new IconRenderLayer
                {
                    Geometry = geometry,
                    Stroke = ResolveBrush(
                        icon.Stroke,
                        defaultBrush),
                    StrokeWidth = icon.StrokeWidth
                }
                ]
            };

            return _normalizer.Normalize(result.Layers);
        }

        private static Brush? ResolveBrush(
            string? value,
            Brush defaultBrush)
        {
            if (value == null)
                return defaultBrush;

            if (string.IsNullOrWhiteSpace(value) ||
                value.Equals("none", StringComparison.OrdinalIgnoreCase))
                return null;

            if (value.Equals(
                "currentColor",
                StringComparison.OrdinalIgnoreCase))
                return defaultBrush;

            return BrushColorHelper.StringFormatToSolidColor(value);
        }
    }
}
