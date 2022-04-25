
namespace UnityCommander.Core.Database.Xml
{
    using System;
    using System.Xml.Linq;

    /// <summary>
    /// The xml writer.
    /// </summary>
    public class XmlWriter : XmlParser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XmlWriter"/> class.
        /// </summary>
        public XmlWriter()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlWriter"/> class.
        /// </summary>
        /// <param name="path"> Path to the xml document. </param>
        /// <param name="elementPointer"> Attribute that points to the node from which traversal of xml elements begins. </param>
        /// <param name="nestedLevel"> Nesting level of the xml elements. </param>
        public XmlWriter(string path, XElement elementPointer, byte nestedLevel) 
            : base(path, nestedLevel)
        {
            this.GetCurrentElement(elementPointer);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlWriter"/> class. 
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
        public XmlWriter(string path, XAttribute attributePointer, byte nestedLevel) 
            : base(path, nestedLevel)
        {
            this.GetCurrentElement(attributePointer);
        }
    }
}
