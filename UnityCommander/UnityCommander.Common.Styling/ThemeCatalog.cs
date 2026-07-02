
using System;
using System.Collections.Generic;
using UnityCommander.Theme;

namespace UnityCommander.Common.Styling
{
    public sealed class ThemeCatalog : IThemeCatalog
    {
        private readonly Dictionary<string, ThemeDefinition>
            _themes =
            new(StringComparer.OrdinalIgnoreCase);

        public ThemeCatalog()
        {
            DarkTheme = new ThemeDefinition
            {
                Priority = 0,
                Name = "Dark",
                ResourceUris =
                    [
                        "/UnityCommander.Common.Styling;component/Themes/DarkTheme.xaml",
                        "/UnityCommander.Ribbon.Wpf;component/Themes/DarkTheme.xaml",
                        "/AvalonDock.Themes.Arc;component/DarkTheme.xaml"
                    ],
                Palette = new ThemePalette
                {
                    Accent = "Theme.Accent",
                    Background = "Theme.Background",
                    Foreground = "Theme.Foreground",

                    Icons = new IconPalette
                    {
                        Folder = "IconFolderBrush",
                        File = "IconFileBrush",
                        Default = "IconDefaultBrush",
                        Muted = "IconMutedBrush",
                        Disabled = "IconDisabledBrush",
                        Accent = "IconAccentBrush",
                        Hover = "IconHoverBrush",
                        Selected = "IconSelectedBrush",
                        Success = "IconSuccessBrush",
                        Warning = "IconWarningBrush",
                        Error = "IconErrorBrush",
                    }
                }
            };

            LightTheme = new ThemeDefinition
            {
                Priority = 0,
                Name = "Light",
                ResourceUris =
                [
                    "/UnityCommander.Common.Styling;component/Themes/LightTheme.xaml",
                    "/UnityCommander.Ribbon.Wpf;component/Themes/LightTheme.xaml",
                    "/AvalonDock.Themes.Arc;component/LightTheme.xaml",
                ],
                Palette = new ThemePalette
                {
                    Accent = "Theme.Accent",
                    Background = "Theme.Background",
                    Foreground = "Theme.Foreground",
                    
                    Icons = new IconPalette
                    {
                        Folder = "IconFolderBrush",
                        File = "IconFileBrush",
                        Default = "IconDefaultBrush",
                        Muted = "IconMutedBrush",
                        Disabled = "IconDisabledBrush",
                        Accent = "IconAccentBrush",
                        Hover = "IconHoverBrush",
                        Selected = "IconSelectedBrush",
                        Success = "IconSuccessBrush",
                        Warning = "IconWarningBrush",
                        Error = "IconErrorBrush",
                    }
                }
            };

            Register(DarkTheme);
            Register(LightTheme);
        }

        public ThemeDefinition DarkTheme { get; }

        public ThemeDefinition LightTheme { get; }

        public IEnumerable<ThemeDefinition> Themes =>
            _themes.Values;

        public void Register(ThemeDefinition theme)
        {
            _themes[theme.Name] = theme;
        }

        public ThemeDefinition Get(string name)
        {
            if (!_themes.TryGetValue(name, out var theme))
            {
                throw new InvalidOperationException(
                    $"Theme '{name}' not found.");
            }

            return theme;
        }

        public bool TryGetTheme(string name, out ThemeDefinition theme)
        {
            return _themes.TryGetValue(name, out theme!);
        }
    }
}
