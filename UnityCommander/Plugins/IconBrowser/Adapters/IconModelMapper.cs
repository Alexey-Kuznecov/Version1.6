
using IconMaker.Core.Helper;
using IconMaker.Core.Models;
using System.Linq;
using System.Windows.Media;
using System.Windows.Shapes;

namespace IconBrowser.Adapters
{
    public static class IconModelMapper
    {
    //    public static IconDefinition ToCore(IconModel model)
    //    {
    //        //return new IconDefinition
    //        //{
    //        //    Name = model.Name,
    //        //    CollectionName = model.CollectionName,
    //        //    Geometry = model.StringPath,
    //        //    Geometries = model.PathList?.Select(p => p.Data.ToString()).ToList(),
    //        //    ForegroundColor = model.FgroundColor.ToString(),
    //        //    BackgroundColor = model.BgroundColor.ToString(),
    //        //    Scale = model.Scale
    //        //};
    //    }

    //    public static IconModel ToLegacy(IconDefinition core)
    //    {
    //        return new IconModel
    //        {
    //            Name = core.Name,
    //            CollectionName = core.CollectionName,
    //            StringPath = core.Geometry,
    //            PathList = core.Geometries?
    //                .Select(g => new Path { Data = Geometry.Parse(g) })
    //                .ToList(),
    //            FgroundColor = core.ForegroundColor.StringFormatToSolidColor(),
    //            BgroundColor = core.BackgroundColor.StringFormatToSolidColor(),
    //            Scale = core.Scale
    //        };
    //    }
    }
}
