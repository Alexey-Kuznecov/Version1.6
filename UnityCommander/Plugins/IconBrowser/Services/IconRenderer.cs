
using IconBrowser.Models;
using IconMaker.Core.Helper;
using IconMaker.Core.Models;
using System.Windows.Media;

namespace IconBrowser.Services
{
    internal class IconRenderer
    {
        public static IconRenderModel Build(IconDefinition icon, IconTheme theme)
        {
            var group = new DrawingGroup();

            foreach (var layer in icon.Layers)
            {
                group.Children.Add(new GeometryDrawing
                {
                    Geometry = Geometry.Parse(layer.Geometry),
                    Brush = layer.Fill.StringFormatToSolidColor()
                });
            }

            return new IconRenderModel
            {
                Id = icon.Id,
                Name = icon.Name,
                Brush = new DrawingBrush(group)
                {
                    Stretch = Stretch.Uniform
                }
            };
        }
    }
}
