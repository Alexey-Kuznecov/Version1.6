
using System.Windows.Media;

namespace IconMaker.Core.Models
{
    public sealed class ColorScheme
    {
        public string Id { get; set; }

        public Color Background { get; set; }
        public Color Foreground { get; set; }

        public Dictionary<string, Color> Accents { get; set; } = new();
    }
}
