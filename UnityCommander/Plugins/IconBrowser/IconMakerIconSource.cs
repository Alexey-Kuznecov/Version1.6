
using IconBrowser.Converters;
using IconMaker.Core.Models;
using IconMaker.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityCommander.Abstractions.Icons;
using UnityCommander.Logging.Contracts;
using UnityCommander.Logging.Core;
using UnityCommander.Logging.Infrastructure;

namespace IconBrowser
{
    public sealed class IconMakerIconSource : IIconSource
    {
        private readonly Dictionary<string, RuntimeIcon> _icons;

        private readonly ILogger _logger;

        public IconMakerIconSource(
            IIconService iconService,
            IconDefinitionCompiler converter, LoggerCreator loggerCreator)
        {
            _logger = loggerCreator.For<IconMakerIconSource>(LogScope.Runtime);

            _icons = new Dictionary<string, RuntimeIcon>(
                StringComparer.OrdinalIgnoreCase);

            foreach (var definition in iconService.GetPack("misk").Icons)
            {
                try
                {
                    var icon = converter.Compile(definition);

                    if (string.IsNullOrWhiteSpace(icon.Key))
                        continue;

                    _icons.TryAdd(icon.Key, icon);
                }
                catch (Exception ex)
                {
                    _logger.Error(
                        $"Failed to compile icon '{definition.Name}'.", ex);
                }
            }
        }

        public int Priority => 100;

        public bool TryGet(
            string key,
            out RuntimeIcon icon)
            => _icons.TryGetValue(key, out icon!);
    }
}
