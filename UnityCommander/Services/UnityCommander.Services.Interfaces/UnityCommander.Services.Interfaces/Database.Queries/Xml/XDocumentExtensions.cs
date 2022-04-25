
namespace UnityCommander.Services.Interfaces.Database.Queries.Xml
{
    using System.Collections.Generic;

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
        public static IEnumerable<XElementInfo> Find(this XDocument document, string name)
        {
            foreach (var elementInfo in document.ElementInfo.ChildrenInfos)
            {
                foreach (var r in elementInfo.FindOf(elementInfo, name))
                {
                    yield return r;
                }
            }
        }
    }
}
