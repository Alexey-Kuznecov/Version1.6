
using System;
using System.Collections.Generic;
using UnityCommander.Core.Theming;

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
                Name = "Material",
                ResourceUris =
                [  
                    "/UnityCommander.Common.Styling;component/Themes/MaterialTheme.xaml",
                ]
            };

            Register(Default);

            Register(
                new ThemeDefinition
                {
                    Name = "Default",
                    ResourceUris =
                    [
                        "/UnityCommander.Common.Styling;component/Themes/DefaultTheme.xaml",
                    ]
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
