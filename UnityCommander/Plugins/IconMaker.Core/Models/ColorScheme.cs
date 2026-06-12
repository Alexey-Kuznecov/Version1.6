
using System.Windows.Media;

namespace IconMaker.Core.Models
{
    public sealed class ColorScheme
    {
        public required string Id { get; set; }

        public required string Name { get; set; }

        public Color PrimaryColor { get; set; }

        public Color SecondaryColor { get; set; }

        public Color AccentColor { get; set; }

        public Color BackgroundColor { get; set; }
    }
}
