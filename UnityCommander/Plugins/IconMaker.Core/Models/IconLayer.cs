
using System.Drawing;

namespace IconMaker.Core.Models
{
    public sealed class IconLayer
    {
        public required string Geometry { get; init; }

        public required string Fill { get; set; }

        public string? Stroke { get; set; }

        public int Order { get; set; }
    }
}
