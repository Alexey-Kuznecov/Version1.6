
using IconMaker.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using UnityCommander.Abstractions.Icons;
using UnityCommander.Logging.Contracts;
using UnityCommander.Logging.Core;
using UnityCommander.Logging.Infrastructure;

namespace IconBrowser.Converters
{
    public sealed class IconDefinitionCompiler
    {
        private readonly ILogger _logger;

        public IconDefinitionCompiler(LoggerCreator loggerCreator)
        {
            _logger = loggerCreator.For<IconDefinitionCompiler>(LogScope.Runtime);
        }

        public RuntimeIcon Compile(IconDefinition definition)
        {
            ArgumentNullException.ThrowIfNull(definition);

            var layers = definition.Layers
                .OrderBy(x => x.Order)
                .Select(x => new RuntimeIconLayer
                {
                    Data = x.Geometry,
                    Fill = x.Fill,
                    Stroke = x.Stroke,
                    StrokeWidth = x.StrokeWidth,
                    StrokeLineCap = x.StrokeLineCap,
                    StrokeLineJoin = x.StrokeLineJoin
                })
                .ToList();

            if (layers.Count == 0)
                _logger.Error($"Icon '{definition.Name}' contains no layers.");

            if (layers.Any(x => string.IsNullOrWhiteSpace(x.Data)))
                _logger.Error($"Icon '{definition.Name}' contains an empty geometry.");

            return new RuntimeIcon
            {
                Data = layers[0].Data,
                Key = definition.Name,
                IconType = ResolveIconType(layers),
                Layers = layers
            };
        }

        private static IconType ResolveIconType(
            IReadOnlyList<RuntimeIconLayer> layers)
        {
            if (layers.Count > 1)
                return IconType.Layered;

            var layer = layers[0];

            if (!string.IsNullOrWhiteSpace(layer.Stroke) ||
                layer.StrokeWidth.HasValue)
                return IconType.Stroke;

            return IconType.Filled;
        }
    }
}
