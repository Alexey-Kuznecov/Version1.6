
using IconMaker.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IconBrowser.Adapters
{
    public class XmlToJsonMigrator
    {
        //public IconPack Convert(List<IconModel> icons)
        //{
        //    //var pack = new IconPack
        //    //{
        //    //    Id = Guid.NewGuid().ToString(),
        //    //    Name = icons.FirstOrDefault()?.CollectionName ?? "Unknown",
        //    //    Icons = new List<IconDefinition>()
        //    //};

        //    //foreach (var icon in icons)
        //    //{
        //    //    pack.Icons.Add(new IconDefinition
        //    //    {
        //    //        Id = Guid.NewGuid(),
        //    //        Name = icon.Name,
        //    //        Scale = icon.Scale,
        //    //        Background = icon.BgroundColor?.Color.ToString(),
        //    //        Foreground = icon.FgroundColor?.Color.ToString(),
        //    //        Layers = icon.PathList?.Select(p => new IconLayer
        //    //        {
        //    //            Geometry = p.Data.ToString(), // или p.Data если есть
        //    //            Fill = icon.Brush.ToString()
        //    //        }).ToList() ?? new()
        //    //    });
        //    //}

        //    return pack;
        //}
    }
}
