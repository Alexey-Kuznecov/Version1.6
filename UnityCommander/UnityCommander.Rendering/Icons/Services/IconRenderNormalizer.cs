
using System.Windows;
using System.Windows.Media;

namespace UnityCommander.Rendering.Icons.Services
{
    public sealed class IconRenderNormalizer : IIconRenderNormalizer
    {
        public IconRenderResult Normalize(
            IReadOnlyList<IconRenderLayer> layers)
        {
            var bounds = GetBounds(layers);

            var normalizedLayers = layers
                .Select(layer => new IconRenderLayer
                {
                    Geometry = NormalizeGeometry(
                        layer.Geometry,
                        bounds),

                    Fill = layer.Fill,
                    Stroke = layer.Stroke,
                    StrokeWidth = layer.StrokeWidth,
                    StrokeLineCap = layer.StrokeLineCap,
                    StrokeLineJoin = layer.StrokeLineJoin,
                    Order = layer.Order
                })
                .ToList();

            return new IconRenderResult
            {
                Layers = normalizedLayers,
                ViewBoxWidth = bounds.Width,
                ViewBoxHeight = bounds.Height
            };
        }

        private static Rect GetBounds(
            IReadOnlyList<IconRenderLayer> layers)
        {
            var bounds = Rect.Empty;

            foreach (var layer in layers)
                bounds.Union(layer.Geometry.Bounds);

            return bounds;
        }

        private static Geometry NormalizeGeometry(
            Geometry geometry,
            Rect bounds)
        {
            var normalized = geometry.Clone();

            normalized.Transform = new TranslateTransform(
                -bounds.X,
                -bounds.Y);

            normalized.Freeze();

            return normalized;
        }
    }
}
