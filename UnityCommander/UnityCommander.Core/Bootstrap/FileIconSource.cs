
using System;
using System.Collections.Generic;
using System.Linq;
using UnityCommander.Abstractions.Icons;
using UnityCommander.Core.Binary;

namespace UnityCommander.Core.Bootstrap
{
    public sealed class FileIconSource : IIconSource
    {
        private readonly Dictionary<string, RuntimeIcon> _icons;

        public FileIconSource(string path)
        {
            _icons = IconPackBinaryReader.Load(path)
             .OrderBy(x => x.Key, StringComparer.OrdinalIgnoreCase)
             .ToDictionary(x => x.Key, x => x.Value, StringComparer.OrdinalIgnoreCase);
        }

        public int Priority => 10;

        public bool TryGet(string key, out RuntimeIcon icon)
            => _icons.TryGetValue(key, out icon!);
    }
}
