
namespace IconMaker.Core.Models
{
    public sealed class IconLayer
    {
        public required string Geometry { get; init; }

        public string? Fill { get; set; }
        public string? Stroke { get; set; }

        public double? StrokeWidth { get; set; }
        public string? StrokeLineCap { get; set; }
        public string? StrokeLineJoin { get; set; }

        public int Order { get; set; }
    }
}
