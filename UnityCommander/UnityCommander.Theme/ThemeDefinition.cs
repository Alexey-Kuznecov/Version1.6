
namespace UnityCommander.Theme
{
    public class ThemeDefinition
    {
        public string Name { get; set; }

        public int Priority { get; init; }

        public IReadOnlyList<string> ResourceUris { get; set; }

        public ThemePalette Palette { get; init; }
    }
}