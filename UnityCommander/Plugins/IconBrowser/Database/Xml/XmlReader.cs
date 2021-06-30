using System.Xml.Linq;

namespace AIconBrowser.Database.Xml
{
    public class XmlReader : XmlParser
    {
        public XmlReader() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlParser"/> class.
        /// </summary>
        /// <param name="path"> Path to the xml document. </param>
        /// <param name="elementPointer"> Attribute that points to the node from which traversal of xml elements begins. </param>
        /// <param name="nestLevel"> Nesting level of the xml elements. </param>
        public XmlReader(string path, XElement elementPointer, byte nestLevel) : base(path, nestLevel)
        {
            base.GetCurrentElement(elementPointer);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlParser"/> class.
        /// </summary>
        /// <param name="path"> Path to the xml document. </param>
        /// <param name="attributePointer"> Attribute that points to the node from which traversal of xml elements begins. </param>
        /// <param name="nestLevel"> Nesting level of the xml elements. </param>
        public XmlReader(string path, XAttribute attributePointer, byte nestLevel) : base(path, nestLevel)
        {
            base.GetCurrentElement(attributePointer);
        }
    }
}
