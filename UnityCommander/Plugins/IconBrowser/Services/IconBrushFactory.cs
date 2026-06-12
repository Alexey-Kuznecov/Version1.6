
using IconMaker.Core.Models;
using System.Linq;
using System.Windows.Media;

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

        private static Brush ParseBrush(string color)
        {
            return (SolidColorBrush)
                new BrushConverter().ConvertFromString(color);
        }
    }
}
