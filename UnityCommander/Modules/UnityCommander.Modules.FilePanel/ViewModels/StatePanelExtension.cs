
namespace UnityCommander.Modules.FilePanel.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using UnityCommander.Services.Interfaces.Database.Queries.Xml;

    /// <summary>
    /// The state panel serializer.
    /// </summary>
    public static class StatePanelExtension
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
        /// <returns>
        /// The collection of <see cref="XElementInfo"/>.
        /// </returns>
        public static List<XElementInfo> GetTabConfigs(this XDocument document)
        {
            // tabsInfos = document.Find("Tab");
            return tabsInfos;
        }

        /// <summary>
        /// The set tab config.
        /// </summary>
        /// <param name="elementInfos">
        /// The document.
        /// </param>
        /// <param name="tabRecord">
        /// The record.
        /// </param>
        /// <param name="panel">
        /// The panel.
        /// </param>
        /// <returns>
        /// The collection of <see cref="XElementInfo"/>.
        /// </returns>
        public static List<XElementInfo> SetTabConfigs(this List<XElementInfo> elementInfos, TabRecord tabRecord, string panel)
        {
            foreach (var info in elementInfos)
            {
            }

            return elementInfos;
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
        /// The collection of <see cref="TabRecord"/>.
        /// </returns>
        public static IEnumerable<TabRecord> GetTabConfigs(this XDocument document, string filter)
        {
            foreach (var element in document.Find("Tab"))
            {
                var parent = element.FindAncestor(element, 2);
                
                if (filter.Contains(parent.Element.FirstAttribute.Value))
                {
                    if (Directory.Exists(element.GetAttributeValueByName("Path")))
                    {
                        var record = new TabRecord
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
