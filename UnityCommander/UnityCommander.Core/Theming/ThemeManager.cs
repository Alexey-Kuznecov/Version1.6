
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

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

        private static void ApplyTheme(ThemeDefinition theme)
        {
            //var appResources = Application.Current.Resources.MergedDictionaries;

            //// 1. Убираем старые theme dictionaries (но не base/framework)
            //var toRemove = appResources
            //    .Where(d => IsThemeDictionary(d))
            //    .ToList();

            //foreach (var dict in toRemove)
            //    appResources.Remove(dict);

            //// 2. Добавляем новый theme в конец (highest priority)
            //foreach (var uri in theme.ResourceUris)
            //{
            //    appResources.Add(new ResourceDictionary
            //    {
            //        Source = new Uri(uri, UriKind.RelativeOrAbsolute)
            //    });
            //}
        }

        private static bool IsThemeDictionary(ResourceDictionary dict)
        {
            return dict.Source != null &&
                   dict.Source.OriginalString.Contains("Themes");
        }
    }
}
