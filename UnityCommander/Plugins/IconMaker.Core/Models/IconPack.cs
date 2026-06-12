
namespace IconMaker.Core.Models
{
    public sealed class IconPack
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public List<IconDefinition> Icons { get; set; } = [];

        public IconPack()
        {
        }

        public IconPack(string id, string name, IEnumerable<IconDefinition>? icons = null)
        {
            Id = id;
            Name = name;
            if (icons != null)
                Icons.AddRange(icons);
        }

        public void AddIcon(IconDefinition icon)
            => Icons.Add(icon);

        public void RemoveIcon(IconDefinition icon)
            => Icons.Remove(icon);

        public void AddRange(List<IconDefinition> icons)
            => Icons.AddRange(icons);
    }
}
