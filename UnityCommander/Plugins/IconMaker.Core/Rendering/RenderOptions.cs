
using IconMaker.Core.Models;
using System.Windows.Media;

namespace IconMaker.Core.Rendering
{
    public sealed class RenderOptions
    {
        public int Scale { get; set; }

        public IconRenderMode Mode { get; set; }

        public Color? MonochromeColor { get; set; }

        public IColorScheme? Scheme { get; set; }
    }
}
