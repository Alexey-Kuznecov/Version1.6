using System;
using System.Collections.Generic;
using System.IO;
using UnityCommander.Services.Interfaces.Database.Queries.Xml;

namespace UnityCommander.Modules.TabPanel
{
    /// <summary>
    /// The state panel serializer.
    /// </summary>
    public static class TabPanelExtension
    {
        /// <summary>
        /// The tabs info.
        /// </summary>
        private static List<XElementInfo> tabsInfos;

        /// <summary>
        /// The xml deserialize.
        /// </summary>
        /// <param name="o">
        /// The o.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool XmlDeserialize(object o)
        {
            return false;
        }

        /// <summary>
        /// The get tab config.
        /// </summary>
        /// <param name="document">
        /// The document.
        /// </param>
        /// <param name="filter">
        /// The filter.
        /// </param>
        /// <returns>
        /// The collection of <see cref="TabPanelRecord"/>.
        /// </returns>
        public static IEnumerable<TabPanelRecord> GetTabConfigs(this XDocument document, string filter)
        {
            foreach (var element in document.Find("Tab"))
            {
                var parent = element.FindAncestor(element, 2);
                
                if (filter.Contains(parent.Element.FirstAttribute.Value))
                {
                    if (Directory.Exists(element.GetAttributeValueByName("Path")))
                    {
                        var record = new TabPanelRecord
                         {
                             Path = element.GetAttributeValueByName("Path"),
                             Token = Guid.Parse(element.GetAttributeValueByName("Id")),
                             Panel = parent.GetAttributeValueByName("Name")
                        };

                        yield return record;
                    }
                }
            }
        }

        /// <summary>
        /// The get paths.
        /// </summary>
        /// <param name="document">
        /// The document.
        /// </param>
        /// <returns>
        /// The D.
        /// </returns>
        public static string[] GetPaths(this XDocument document)
        {
            int counter = 0;

            string[] paths = new string[tabsInfos.Count];

            foreach (var info in document.Find("Tab"))
            {
                foreach (var attribute in info.Attributes)
                {
                    if (attribute.Name == "Path")
                    {
                        paths[counter++] = attribute.Value;
                    }
                }
            }

            return paths;
        }
    }
}
