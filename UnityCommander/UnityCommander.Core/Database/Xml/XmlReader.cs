
namespace UnityCommander.Core.Database.Xml
{
    using System.Xml.Linq;

    /// <summary>
    /// The xml reader.
    /// </summary>
    public class XmlReader : XmlParser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XmlReader"/> class.
        /// </summary>
        public XmlReader()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlReader"/> class. 
        /// </summary>
        /// <param name="path">
        /// Path to the xml document. 
        /// </param>
        /// <param name="elementPointer">
        /// Attribute that points to the node from which traversal of xml elements begins. 
        /// </param>
        /// <param name="nestedLevel">
        /// Nesting level of the xml elements. 
        /// </param>
        public XmlReader(string path, XElement elementPointer, byte nestedLevel) : base(path, nestedLevel)
        {
            this.GetCurrentElement(elementPointer);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlReader"/> class. 
        /// </summary>
        /// <param name="path">
        /// Path to the xml document. 
        /// </param>
        /// <param name="attributePointer">
        /// Attribute that points to the node from which traversal of xml elements begins. 
        /// </param>
        /// <param name="nestedLevel">
        /// Nesting level of the xml elements. 
        /// </param>
        public XmlReader(string path, XAttribute attributePointer, byte nestedLevel) 
            : base(path, nestedLevel)
        {
            this.GetCurrentElement(attributePointer);
        }
    }
}

