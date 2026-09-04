
using IconBrowser.Models;
using IconMaker.Core.Helper;
using IconMaker.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Xml.Linq;

namespace IconBrowser.Services
{
    public static class IconBrushFactory
    {
        public static DrawingBrush Create(IconDefinition icon)
        {
            var group = new DrawingGroup();

            foreach (var layer in icon.Layers.OrderBy(x => x.Order))
            {
                var drawing = new GeometryDrawing
                {
                    Geometry = Geometry.Parse(layer.Geometry)
                };

                if (!string.IsNullOrWhiteSpace(layer.Fill))
                {
                    drawing.Brush = ParseBrush(layer.Fill);
                }

                if (!string.IsNullOrWhiteSpace(layer.Stroke))
                {
                    var pen = new Pen(
                        ParseBrush(layer.Stroke),
                        layer.StrokeWidth ?? 1);

                    drawing.Pen = pen;
                }

                group.Children.Add(drawing);
            }

            return new DrawingBrush
            {
                Drawing = group,
                Stretch = Stretch.Uniform
            };
        }

        public static DrawingBrush CreateBrush(
          IconDefinition icon,
          IconTheme theme)
        {
            var group = new DrawingGroup();

            foreach (var layer in icon.Layers.OrderBy(x => x.Order))
            {
                var geometryDrawing = new GeometryDrawing
                {
                    Geometry = Geometry.Parse(layer.Geometry)
                };

                if (theme.IsMonochrome)
                {
                    var color =
                        theme.MonochromeColor.StringFormatToSolidColor();

                    if (layer.Fill != null)
                        geometryDrawing.Brush = color;

                    if (layer.Stroke != null)
                    {
                        geometryDrawing.Pen = new Pen(
                            color,
                            layer.StrokeWidth ?? 1);
                    }
                }
                else
                {
                    if (layer.Fill != null)
                    {
                        geometryDrawing.Brush =
                            ResolveColor(layer.Fill, theme);
                    }

                    if (layer.Stroke != null)
                    {
                        geometryDrawing.Pen = new Pen(
                            ResolveColor(layer.Stroke, theme),
                            layer.StrokeWidth ?? 1);
                    }
                }

                group.Children.Add(geometryDrawing);
            }

            return new DrawingBrush
            {
                Drawing = group,
                Stretch = Stretch.Uniform
            };
        }

        private static SolidColorBrush ResolveColor(
            string value,
            IconTheme theme)
        {
            if (value.Equals(
                    "currentColor",
                    StringComparison.OrdinalIgnoreCase))
            {
                return theme.MonochromeColor
                    .StringFormatToSolidColor();
            }

            return value.StringFormatToSolidColor();
        }

        private static SolidColorBrush ParseBrush(string color)
        {
            return (SolidColorBrush)
                new BrushConverter().ConvertFromString(color)!;
        }
    }
}
