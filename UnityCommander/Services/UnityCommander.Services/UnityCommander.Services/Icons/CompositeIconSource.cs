
using System;
using System.Collections.Generic;
using System.Linq;
using UnityCommander.Abstractions.Icons;
using UnityCommander.Logging.Contracts;
using UnityCommander.Logging.Core;
using UnityCommander.Logging.Infrastructure;
using UnityCommander.Rendering.Icons;

namespace UnityCommander.Abstractions.Resources
{
    public sealed class CompositeIconResolver : IIconResolver
    {
        private readonly Dictionary<string, RuntimeIcon> _cache = new();

        private readonly HashSet<string> _missing = new();

        private readonly IIconSourceRegistry _registry;

        private RuntimeIcon _missingIcon = DefaultIcons.Missing;

        private readonly CompositeIconResolver _iconResolver;

        private readonly ILogger _logger;

        public CompositeIconResolver(IIconSourceRegistry iconSource, LoggerCreator loggerCreator)
        {
            _logger = loggerCreator.For<CompositeIconResolver>(LogScope.Runtime);

            _registry = iconSource;
        }

        public bool TryResolve(string key, out RuntimeIcon icon)
        {
            if (_cache.TryGetValue(key, out icon))
                return true;

            if (_missing.Contains(key))
            {
                icon = _missingIcon!;
                return false;
            }

            foreach (var source in _registry.Sources.OrderByDescending(x => x.Priority))
            {
                try
                {
                    if (!source.TryGet(key, out var resolvedIcon))
                        continue;

                    if (resolvedIcon is null)
                    {
                        _logger.Warning(
                            $"Icon source '{source.GetType().FullName}' returned null " +
                            $"for icon '{key}'.");

                        continue;
                    }

                    _cache[key] = resolvedIcon;
                    icon = resolvedIcon;
                    return true;
                }
                catch (Exception ex)
                {
                    _logger.Error(
                        $"Icon source '{source.GetType().FullName}' failed to resolve icon '{key}'.", ex);
                }
            }

            _missing.Add(key);
            icon = _missingIcon!;
            return false;
        }

        public RuntimeIcon Resolve(string key)
        {
            try
            {
                if (_cache.TryGetValue(key, out var icon))
                    return icon;

                if (TryResolve(key, out icon))
                    return icon;

                return _missingIcon;
            }
            catch (ArgumentNullException ex)
            {
                _logger.Error(
                   
                    $"Failed to resolve icon '{key}'.", ex);

                return _missingIcon;
            }
        }
    }
}
