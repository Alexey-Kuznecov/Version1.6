
// ReSharper disable All
namespace IconBrowser.Converters
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Windows.Media;
    using System.Windows.Shapes;

    [DebuggerStepThrough]
    class ConverterXamlResources
    {
        /// <summary>
        /// Преобразует значение свойства Path.Data в свойство GeometryDrawing.Geometry,
        /// возвращает коллекцию геометрических точек из которых состоит иконка.
        /// </summary>
        /// <param name="paths">Принимает коллекцию из объектов Path.</param>
        /// <returns>Возвращает коллекцию из объектов GeometryDrawing.</returns>
        public static List<GeometryDrawing> ConvertDataToGeometry(List<Path> paths)
        {
            List<GeometryDrawing> geometry = new List<GeometryDrawing>();
            foreach (Path item in paths)
            {
                string sub = item.Data.ToString().Substring(2).Replace(',', '.').Replace(';', ',');
                geometry.Add(new GeometryDrawing { Geometry = Geometry.Parse(sub) });
            }
            return geometry;
        }
        /// <summary>
        /// Упаковывает геометрию иконки в кисть, которая соответствует требованию разметки словоря иконок. 
        /// </summary>
        /// <summary>Редактор иконок требует такую разметку для коректной загрузки и обработки иконок. </summary>
        /// <param name="geometry">Коллекция из объектов GeometryDrawing.</param>
        /// <returns>Возвращает геометрию иконки как кисть.</returns>
        public static DrawingBrush ConvertMarkupDrawingBrush(List<GeometryDrawing> geometry)
        {
            DrawingBrush drawBrush = new DrawingBrush();
            DrawingGroup group = new DrawingGroup();

            foreach (var brush in geometry)
                group.Children.Add(brush);
            drawBrush.Drawing = group;
            return drawBrush;
        }
    }
}
