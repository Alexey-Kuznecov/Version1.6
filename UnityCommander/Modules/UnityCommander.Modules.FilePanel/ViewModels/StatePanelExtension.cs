
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
        /// The xml serializer.
        /// </summary>
        /// <param name="o">
        /// The o.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool XmlSerialize(object o)
        {
            XDocument document = new XDocument("app.session", "AppSession");
            document.SerializeObject("FilePanel", o);
            var ddd = document.SearchElement("Icon");
            return false;
        }

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
            tabsInfos = document.SearchElement("Tab");
            return tabsInfos;
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
            tabsInfos = document.SearchElement("Tab");

            foreach (var item in tabsInfos)
            {
                var parent = item.FindAncestor(item, 2);
                
                if (filter.Contains(parent.Element.FirstAttribute.Value))
                {
                    if (Directory.Exists(item.Attributes[1].Value))
                    {
                        var record = new TabRecord
                         {
                             Path = item.Attributes[1].Value,
                             Token = Guid.Parse(item.Attributes[0].Value),
                             Panel = parent.Attributes[0].Value
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

            if (tabsInfos == null)
            {
                tabsInfos = document.SearchElement("Tab");
            }

            string[] paths = new string[tabsInfos.Count];

            foreach (var info in tabsInfos)
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
