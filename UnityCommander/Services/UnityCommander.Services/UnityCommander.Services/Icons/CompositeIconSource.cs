
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Shapes;
using UnityCommander.Common.Models.Icons;
using UnityCommander.Core.Resources;

namespace UnityCommander.Abstractions.Resources
{
    public sealed class CompositeIconResolver
     : IIconResolver
    {
        private readonly Dictionary<string, IIcon> _cache = new();

        private readonly IIconSourceRegistry _registry;

        private IIcon _missingIcon;

        public CompositeIconResolver(IIconSourceRegistry iconSource)
        {
            _registry = iconSource;
        }

        public bool TryResolve(string key, out IIcon icon)
        {
            if (_cache.TryGetValue(key, out icon))
                return true;

            foreach (var source in _registry.Sources)
            {
                if (!source.TryGet(key, out var definition))
                    continue;

                icon = Convert(definition);

                _cache[key] = icon;

                return true;
            }

            icon = _missingIcon;
            _cache[key] = icon;

            return false;
        }

        public IIcon Resolve(string key)
        {
            if (_cache.TryGetValue(key, out var icon))
                return icon;

            if (TryResolve(key, out icon))
                return icon;

            return _missingIcon;
        }

        private IIcon Convert(
            IconDefinition definition)
        {
            return new Icon
            {
                Path = new Path
                {
                    Data = Geometry.Parse(
                        definition.Data)
                }
            };
        }
    }
}
