
namespace UnityCommander.Theme
{
    public sealed class ThemePalette
    {
        public string? Background { get; init; }
        public string? BackgroundSecondary { get; init; }

        public string? Foreground { get; init; }
        public string? ForegroundSecondary { get; init; }

        public string? Accent { get; init; }
        
        public string? AccentHover { get; init; }

        public string? Border { get; init; }

        public IconPalette Icons { get; init; } = new();
    }
}
