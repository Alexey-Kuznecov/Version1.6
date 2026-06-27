
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
            Default = new ThemeDefinition
            {
                Priority = 0,
                Name = "Default",
                ResourceUris =
                    [
                        "/UnityCommander.Common.Styling;component/Themes/DefaultTheme.xaml",
                    ],
                Palette = new ThemePalette
                {
                    Accent = "Theme.Accent",
                    Background = "Theme.Background",
                    Foreground = "Theme.Foreground",

                    Icons = new IconPalette
                    {
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

            Register(Default);

            Register(new ThemeDefinition
            {
                Priority = 0,
                Name = "Material",
                ResourceUris =
                [
                    "/UnityCommander.Common.Styling;component/Themes/MaterialTheme.xaml",
                ],
                Palette = new ThemePalette
                {
                    Accent = "Theme.Accent",
                    Background = "Theme.Background",
                    Foreground = "Theme.Foreground",

                    Icons = new IconPalette
                    {
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
            });
        }

        public ThemeDefinition Default { get; }

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
