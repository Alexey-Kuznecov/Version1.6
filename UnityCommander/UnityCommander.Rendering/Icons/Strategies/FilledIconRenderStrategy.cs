
using System.Windows;
using System.Windows.Media;
using UnityCommander.Abstractions.Icons;
using UnityCommander.Diagnostics.Tracing;
using UnityCommander.Rendering.Converters;

namespace UnityCommander.Rendering.Icons.Strategies
{
    public sealed class FilledIconRenderStrategy : IIconRenderStrategy
    {
        public readonly IDiagnosticTrace _trace;

        public IconType Type => IconType.Filled;

        public FilledIconRenderStrategy(IDiagnosticTrace trace)
        {
            _trace = trace;
        }

        public IconRenderResult Render(
            RuntimeIcon icon,
            Brush defaultBrush)
        {
            var geometry = Geometry.Parse(icon.Data!);
            geometry.Freeze();

            var bounds = geometry.Bounds;

            _trace.Write(
                "icon.render",
                "bounds",
                DiagnosticTraceData.Of(
                    ("x", bounds.X),
                    ("y", bounds.Y),
                    ("width", bounds.Width),
                    ("height", bounds.Height)));

            var normalized = geometry.Clone();

            normalized.Transform = new TranslateTransform(
                -bounds.X,
                -bounds.Y);

            normalized.Freeze();

            return new IconRenderResult
            {
                Layers =
                [
                    new IconRenderLayer
            {
                Geometry = normalized,
                Fill = ResolveBrush(icon.Color, defaultBrush),
                Order = 0
            }
                ],
                ViewBoxWidth = bounds.Width,
                ViewBoxHeight = bounds.Height
            };
        }

        private static Brush? ResolveBrush(
           string? value,
           Brush defaultBrush)
        {
            if (defaultBrush != null)
                return defaultBrush;

            if (value.Equals(
                "none",
                StringComparison.OrdinalIgnoreCase))
                return null;

            if (value.Equals(
                "currentColor",
                StringComparison.OrdinalIgnoreCase))
                return defaultBrush;

            return BrushColorHelper.StringFormatToSolidColor(value);
        }
    }
}
