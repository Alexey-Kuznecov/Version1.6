
using IconMaker.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace IconBrowser.Models
{
    public class XmlIconReader
    {
        public List<IconDefinition> Read(string file, string collectionName)
        {
            XElement root = XElement.Load(file);

            var result = new List<IconDefinition>();

            var collections = root.Elements()
                .Where(x => x.FirstAttribute.Value == collectionName);

            foreach (var collection in collections)
            {
                foreach (var element in collection.Elements())
                {
                    result.Add(new IconDefinition
                    {
                        Id = Guid.NewGuid(),
                        Name = element.Attribute("Name")?.Value,
                        Scale = int.Parse(element.Attribute("Scale")?.Value ?? "64"),
                        Background = element.Attribute("Background")?.Value,
                        Foreground = element.Attribute("Foreground")?.Value,
                        Layers = element.Elements("Path")
                            .Select(p => new IconLayer
                            {
                                Geometry = p.Value,
                                Fill = p.Attribute("Fill")?.Value.ToString()
                            })
                            .ToList()
                    });
                }
            }

            return result;
        }
    }
}
