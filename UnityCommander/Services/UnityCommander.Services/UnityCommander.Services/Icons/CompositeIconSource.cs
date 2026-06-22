
using System.Collections.Generic;
using System.Linq;
using UnityCommander.Rendering.Icons;

namespace UnityCommander.Abstractions.Resources
{
    public sealed class CompositeIconResolver : IIconResolver
    {
        private readonly Dictionary<string, RuntimeIcon> _cache = new();

        private readonly HashSet<string> _missing = new();

        private readonly IIconSourceRegistry _registry;

        private RuntimeIcon _missingIcon = new RuntimeIcon();

        public CompositeIconResolver(IIconSourceRegistry iconSource)
        {
            _registry = iconSource;
        }

        public bool TryResolve(string key, out RuntimeIcon icon)
        {
            if (_cache.TryGetValue(key, out icon))
                return true;

            if (_missing.Contains(key))
            {
                icon = default!;
                return false;
            }

            foreach (var source in _registry.Sources.OrderByDescending(x => x.Priority))
            {
                if (!source.TryGet(key, out icon))
                    continue;

                _cache[key] = icon;
                return true;
            }

            _missing.Add(key);
            icon = default!;
            return false;
        }

        public RuntimeIcon Resolve(string key)
        {
            if (_cache.TryGetValue(key, out var icon))
                return icon;

            if (TryResolve(key, out icon))
                return icon;

            return _missingIcon;
        }
    }
}
