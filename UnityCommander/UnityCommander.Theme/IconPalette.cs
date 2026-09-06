
namespace UnityCommander.Theme
{
    public sealed class IconPalette
    {
        public string? Default { get; init; }
        public string? Folder { get; init; }
        public string? File { get; init; }
        public string? Muted { get; init; }
        public string? Disabled { get; init; }
        public string? Accent { get; init; }
        public string? Hover { get; init; }
        public string? Selected { get; init; }
        public string? Error { get; init; }
        public string? Warning { get; init; }
        public string? Success { get; init; }
        public string? Drive { get; set; }
        public string? Archive { get; set; }
        public string? Image { get; set; }
        public object? Pressed { get; set; }
        public object? Plugin { get; set; }
    }
}
