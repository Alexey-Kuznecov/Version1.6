using System.Windows.Media;

namespace UnityCommander.Rendering.Icons
{
    public sealed class IconRenderResult
    {
        public required Geometry Geometry { get; init; }

        public Brush? Brush { get; init; }

        public Brush? Stroke { get; init; }

        public double? StrokeWidth { get; init; }

        public int Size { get; init; }
    }
}