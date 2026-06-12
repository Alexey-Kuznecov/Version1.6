
using IconMaker.Core.Helper;
using IconMaker.Core.Models;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace IconBrowser.Adapters
{
    public static class IconWpfMapper
    {
        //public static DrawingBrush BuildBrush(IconDefinition icon)
        //{
        //    var brush = (DrawingBrush)Application.Current.TryFindResource(icon.Name);
        //    var cloned = brush?.Clone();

        //    if (cloned?.Drawing is DrawingGroup group)
        //    {
        //        foreach (var child in group.Children)
        //        {
        //            if (child is GeometryDrawing geometry)
        //            {
        //                geometry.Brush = icon.ForegroundColor.StringFormatToSolidColor();
        //            }
        //        }
        //    }

        //    return cloned;
        //}

        //public static List<Path> BuildPaths(IconDefinition icon)
        //{
        //    var result = new List<Path>();

        //    foreach (var geo in icon.Geometries ?? new())
        //    {
        //        result.Add(new Path
        //        {
        //            Data = Geometry.Parse(geo)
        //        });
        //    }

        //    return result;
        //}
    }
}
