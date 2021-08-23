
namespace AIconBrowser.Contracts
{
    using System;
    using System.Xml.Linq;

    /// <summary>
    /// The XmlReader interface.
    /// </summary>
    public interface IXmlReader
    {
        /// <summary>
        /// The method to read xml document using <see cref="Database.Xml.XmlParser"/> class.
        /// </summary>
        /// <param name="name">
        /// Create a new object and pass an XElement of type as parameters. 
        /// Be sure to include the name of the existing item.
        /// Also indicate the value of the element if you want to refine the selection.
        /// </param>
        /// <param name="attribute">
        /// Create a new object and pass an <see cref="XAttribute"/> of type as parameters. 
        /// Be sure to include the attribute name of the existing item.
        /// Also specify the attribute value if you want to refine the selection.
        /// </param>
        /// <param name="nested">
        /// Specify the nesting level of the element.
        /// </param>
        /// <param name="action">
        /// Pass a pointer to the method(s) that will receive the result. 
        /// </param>
        void XmlReader(XElement name, XAttribute attribute, byte nested, Action<XElement> action);

        /// <summary>
        /// The method applies an <paramref name="action"/> to each element of the xml document.
        /// The action is passed as method parameters.
        /// </summary>
        /// <param name="action"> Pass a pointer to the method(s) that will receive the result. </param>
        void ApplyForEach(Action<XElement> action);
    }
}
