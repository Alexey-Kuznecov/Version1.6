
namespace UnityCommander.Core.Database.Xml
{
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
        /// <returns>
        /// The <see cref="XElementInfo"/>.
        /// </returns>
        public static XElementInfo SearchElement(this XDocument document)
        {
            //foreach (var VARIABLE in document.ChildrenInfo)
            //{  
            //}

            return null;
        }
    }
}
