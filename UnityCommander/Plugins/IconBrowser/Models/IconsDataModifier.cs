
using System;
using System.Linq;
using System.Xml.Linq;

namespace AIconBrowser.Models
{
    /// <summary>
    /// The icons data modifier.
    /// </summary>
    public class IconsDataModifier
    {
        /// <summary>
        /// The document name.
        /// </summary>
        private const string DocumentName = @"d:\Resources\IconsData.xml";

        /// <summary>
        /// The load document.
        /// </summary>
        /// <param name="root"> The root. </param>
        /// <exception cref="ArgumentNullException"> </exception>
        public static void LoadDocument(ref XElement root)
        {
            if (root == null)
            {
                throw new ArgumentNullException(nameof(root));
            }

            root = XElement.Load(DocumentName);
        }

        /// <summary>
        /// Removes the icon from collection using id which passed in argument.
        /// The second argument needs for specific query.
        /// </summary>
        /// <param name="id">Every icon have id in collection,
        /// will be compared with id property of object IconModel.</param>
        /// <param name="collectName">Collection name.</param>
        public static void Remove(int id, string collectName)
        {
            XElement root = new XElement(new XElement("d"));
            LoadDocument(ref root);

            // Find collection by name.
            var collectionName = from coll in root.Elements()
                                 where coll.Attribute("Name")?.Value == collectName
                                 select coll;
            
            // Find icon by id.
            var iconId = from icon in collectionName.Elements()
                where icon.Attribute("Id")?.Value == id.ToString()
                select icon;
            iconId.Remove();
            root.Save(DocumentName);
        }
    }
}
