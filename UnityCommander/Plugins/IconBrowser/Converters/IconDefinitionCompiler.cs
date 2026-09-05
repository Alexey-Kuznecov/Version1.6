
using IconMaker.Core.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using UnityCommander.Abstractions.Icons;

namespace IconBrowser.Converters
{
    public sealed class IconDefinitionCompiler
    {
        public RuntimeIcon Compile(IconDefinition definition)
        {
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

            return new RuntimeIcon
            {
                Data = layers.First().Data,
                Key = definition.Name,
                IconType = ResolveIconType(
                    layers),
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
