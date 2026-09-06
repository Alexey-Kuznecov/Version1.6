
using System.Windows.Media;

namespace UnityCommander.Rendering.Icons
{
    public sealed class IconRenderLayer
    {
        public required Geometry Geometry { get; init; }

        public Brush? Fill { get; init; }

        public Brush? Stroke { get; init; }

        public double? StrokeWidth { get; init; }

        public PenLineCap? StrokeLineCap { get; init; }

        public PenLineJoin? StrokeLineJoin { get; init; }
        public int Order { get; internal set; }
    }
}
