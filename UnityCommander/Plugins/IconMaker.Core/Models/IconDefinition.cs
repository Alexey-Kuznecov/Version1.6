
namespace IconMaker.Core.Models
{
    public class IconDefinition
    {
        public required Guid Id { get; set; }
        public required string Name { get; set; }
        public required int Scale { get; set; }
        public required string Background { get; set; }
        public required string Foreground { get; set; }
        public object? Tags { get; set; }

        public required List<IconLayer> Layers { get; set; }
    }
}
