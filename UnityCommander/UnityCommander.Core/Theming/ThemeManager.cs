
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace UnityCommander.Core.Theming
{
    public static class ThemeManager
    {
        private static IThemeCatalog _catalog;

        public static ThemeDefinition? CurrentTheme
        {
            get;
            private set;
        }

        public static event Action<ThemeDefinition>? ThemeChanged;

        public static void Initialize(IThemeCatalog catalog, string themeName)
        {
            _catalog = catalog;
            CurrentTheme = _catalog.Get(themeName);
        }

        public static void SetTheme(string name)
        {
            var theme = _catalog.Get(name);

            if (ReferenceEquals(CurrentTheme, theme))
                return;

            ApplyTheme(theme);

            CurrentTheme = theme;

            ThemeChanged?.Invoke(theme);
        }

        public static IEnumerable<string> GetResourceUris()
        {
            return _catalog.Default.ResourceUris
                .Concat(
                    ReferenceEquals(CurrentTheme, _catalog.Default)
                        ? Enumerable.Empty<string>()
                        : CurrentTheme.ResourceUris);
        }

        private static void ApplyTheme(
            ThemeDefinition theme)
        {
        }
    }
}
