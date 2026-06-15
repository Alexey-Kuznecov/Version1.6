
using IconBrowser.Models;
using IconMaker.Core.Helper;
using IconMaker.Core.Models;
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
                group.Children.Add(
                    new GeometryDrawing
                    {
                        Geometry = Geometry.Parse(layer.Geometry),
                        Brush = ParseBrush(layer.Fill)
                    });
            }

            return new DrawingBrush
            {
                Drawing = group,
                Stretch = Stretch.Uniform
            };
        }

        public static DrawingBrush CreateBrush(IconDefinition icon, IconTheme theme)
        {
            DrawingBrush dBrush = new DrawingBrush();
            DrawingGroup group = new DrawingGroup();

            foreach (var layer in icon.Layers)
            {
                GeometryDrawing geometryDrawing = new GeometryDrawing();
                geometryDrawing.Geometry = Geometry.Parse(layer.Geometry);
                
                if (theme.IsMonochrome)
                {
                    geometryDrawing.Brush =
                        theme.MonochromeColor.StringFormatToSolidColor();
                }
                else
                {
                    geometryDrawing.Brush =
                        layer.Fill.StringFormatToSolidColor();
                }

                group.Children.Add(geometryDrawing);
            }

            dBrush.Drawing = group;
            dBrush.Stretch = Stretch.Uniform;
            return dBrush;
        }

        private static Brush ParseBrush(string color)
        {
            return (SolidColorBrush)
                new BrushConverter().ConvertFromString(color);
        }
    }
}
