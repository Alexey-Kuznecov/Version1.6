
namespace UnityCommander.Abstractions.Icons
{
    public sealed class RuntimeIcon
    {
        public string? Key { get; init; }
        public string? Data { get; init; }

        public string? Color { get; init; }

        public string? Stroke { get; init; }
        public double? StrokeWidth { get; init; }

        public List<RuntimeIconLayer> Layers { get; init; } = [];
    }
}
