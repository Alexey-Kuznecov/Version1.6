
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using UnityCommander.Abstractions.Icons;
using UnityCommander.Rendering.Converters;
using UnityCommander.Ribbon.Services.Icon;

namespace UnityCommander.Modules.ToolBar.Builder
{
    public sealed class RuntimeIconConverter
    {
        public IconDefinition Convert(RuntimeIcon icon, string key)
        {
            var layers = new List<IconLayer>();

            // Новый формат
            if (icon.Layers.Count > 0)
            {
                layers.AddRange(
                    icon.Layers
                        .Select((layer, index) => new IconLayer
                        {
                            Geometry = Geometry.Parse(layer.Data),
                            Fill = ResolveBrush(layer.Fill),
                            Stroke = ResolveBrush(layer.Stroke),
                            //StrokeWidth = layer.StrokeWidth,
                            //StrokeLineCap = layer.StrokeLineCap,
                            //StrokeLineJoin = layer.StrokeLineJoin,
                            Order = index
                        }));
            }
            // Старый формат
            else if (!string.IsNullOrWhiteSpace(icon.Data))
            {
                layers.Add(new IconLayer
                {
                    Geometry = Geometry.Parse(icon.Data),
                    Fill = ResolveBrush(icon.Color),
                    Stroke = ResolveBrush(icon.Stroke),
                    //StrokeWidth = icon.StrokeWidth,
                    Order = 0
                });
            }

            return new IconDefinition(
                key,
                300,
                250,
                layers);
        }

        private static Brush? ResolveBrush(string? value)
        {
            if (string.IsNullOrWhiteSpace(value) ||
                value.Equals("none", StringComparison.OrdinalIgnoreCase))
                return null;

            if (value.Equals("currentColor", StringComparison.OrdinalIgnoreCase))
                return null;

            return BrushColorHelper.StringFormatToSolidColor(value);
        }
    }
}
