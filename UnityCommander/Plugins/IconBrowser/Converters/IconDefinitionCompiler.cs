
using IconMaker.Core.Models;
using System;
using System.Linq;
using UnityCommander.Abstractions.Icons;

namespace IconBrowser.Converters
{
    public sealed class IconDefinitionCompiler
    {
        public RuntimeIcon Compile(IconDefinition definition)
        {
            return new RuntimeIcon
            {
                Key = definition.Name,

                Layers = definition.Layers
                    .OrderBy(x => x.Order)
                    .Select(x => new RuntimeIconLayer
                    {
                        Data = x.Geometry,
                        Fill = x.Fill,
                        Stroke = x.Stroke,
                        StrokeWidth = x.StrokeWidth,
                        //StrokeLineCap = x.StrokeLineCap,
                        //StrokeLineJoin = x.StrokeLineJoin
                    })
                    .ToList()
            };
        }
    }
}
