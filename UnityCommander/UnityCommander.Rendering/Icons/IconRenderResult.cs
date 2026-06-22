using System.Windows.Media;

namespace UnityCommander.Rendering.Icons
{
    public sealed class IconRenderResult
    {
        public static IconRenderResult Empty { get; }
            = new IconRenderResult();
        public Geometry Geometry { get; init; } = Geometry.Empty;

        public Brush Brush { get; init; } = Brushes.Black;

        public double Size { get; init; }
    }
}