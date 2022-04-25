
namespace UnityCommander.Core.Database.Xml
{
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// The x document extensions.
    /// </summary>
    public static class XDocumentExtensions
    {
        /// <summary>
        /// The add.
        /// </summary>
        /// <param name="document">
        /// The document.
        /// </param>
        /// <param name="elementName">
        /// The document type.
        /// </param>
        public static void Add(this XDocument document, string elementName)
        {
            //foreach (var element in document.ChildrenInfo)
            //{
            //    if (element.Name == elementName)
            //    {
            //        element.Children.Add(null);
            //    }
            //}
        }

        /// <summary>
        /// The object serializer.
        /// </summary>
        /// <param name="document">
        /// The document.
        /// </param>
        /// <param name="elementName">
        /// The element name.
        /// </param>
        /// <param name="data">
        /// The data.
        /// </param>
        public static void SerializeObject(this XDocument document, string elementName, object data)
        {
            //XElementInfo info = new XElementInfo() 

            //PropertyInfo[] propertyInfos = data.GetType().GetProperties();

            //foreach (var propertyInfo in propertyInfos)
            //{
            //    propertyInfo.Name
            //}

            //document
        }

        /// <summary>
        /// The search element.
        /// </summary>
        /// <param name="document">
        /// The document.
        /// </param>
        /// <param name="name">
        /// The Name.
        /// </param>
        /// <returns>
        /// The <see cref="XElementInfo"/>.
        /// </returns>
        public static List<XElementInfo> SearchElement(this XDocument document, string name)
        {
            List<XElementInfo> infos = new List<XElementInfo>();

            foreach (var elementInfo in document.ElementInfo.ChildrenInfos)
            {
                foreach (var r in elementInfo.FindOf(elementInfo, name))
                {
                    infos.Add(r);
                }
            }

            return infos;
        }
    }
}
