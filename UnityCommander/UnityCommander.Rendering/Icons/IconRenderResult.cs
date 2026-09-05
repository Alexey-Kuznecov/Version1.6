using System.Windows.Media;

namespace UnityCommander.Rendering.Icons
{
    public sealed class IconRenderResult
    {
        public required IReadOnlyList<IconRenderLayer> Layers { get; init; }

        public double ViewBoxWidth { get; set; }
        public double ViewBoxHeight { get; set; }

        public double ViewBoxX { get; set; }
        public double ViewBoxY { get; set; }
    }
}