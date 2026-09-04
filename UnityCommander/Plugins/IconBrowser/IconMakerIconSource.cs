
using IconBrowser.Converters;
using IconMaker.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityCommander.Abstractions.Icons;

namespace IconBrowser
{
    public sealed class IconMakerIconSource : IIconSource
    {
        private readonly Dictionary<string, RuntimeIcon> _icons;

        public IconMakerIconSource(
            IIconService iconService,
            IconDefinitionCompiler converter)
        {
            var icons = iconService
                .GetPack("misk")
                .Icons
                .Select(converter.Compile)
                .Where(x => x.Key is not null)
                .ToList();

            _icons = new Dictionary<string, RuntimeIcon>(
                StringComparer.OrdinalIgnoreCase);

            foreach (var icon in icons)
            {
                if (!_icons.ContainsKey(icon.Key!))
                    _icons.Add(icon.Key!, icon);
            }
        }

        public int Priority => 100;

        public bool TryGet(
            string key,
            out RuntimeIcon icon)
            => _icons.TryGetValue(key, out icon!);
    }
}
