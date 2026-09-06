
namespace UnityCommander.Abstractions.Icons
{
    public sealed class RuntimeIconLayer
    {
        public required string Data { get; init; }

        public string? Fill { get; init; }
        public string? Stroke { get; init; }

        public double? StrokeWidth { get; init; }

        public string? StrokeLineCap { get; init; }
        public string? StrokeLineJoin { get; init; }
    }
}
