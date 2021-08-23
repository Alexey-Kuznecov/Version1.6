
namespace UnityCommander.Core.Database.Xml
{
    using System.Collections.Generic;
    using System.Xml.Linq;

    /// <summary>
    /// The element info.
    /// </summary>
    public class XElementInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XElementInfo"/> class.
        /// </summary>
        public XElementInfo()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XElementInfo"/> class.
        /// </summary>
        /// <param name="element">
        /// The element.
        /// </param>
        public XElementInfo(XElement element)
        {
            this.Element = element;
            this.Name = element.Name.LocalName;

            if (element.FirstAttribute != null)
            {
                this.Attributes.Add(element.FirstAttribute);
            }

            if (element.LastAttribute != null)
            {
                this.Attributes.Add(element.LastAttribute);
            }
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the element.
        /// </summary>
        public XElement Element { get; set; }

        /// <summary>
        /// Gets or sets the current path.
        /// </summary>
        public XPath CurrentPath { get; set; }

        /// <summary>
        /// Gets or sets the attributes.
        /// </summary>
        public List<XAttribute> Attributes { get; set; } = new ();

        /// <summary>
        /// Gets or sets the has elements list.
        /// </summary>
        public List<XElement> Children { get; set; } = new ();

        /// <summary>
        /// Gets the children info.
        /// </summary>
        public XElementInfo ChildrenInfo { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether has elements.
        /// </summary>
        public bool HasElements => this.Children.Count > 0;

        /// <summary>
        /// Gets or sets the position of the current item.
        /// </summary>
        public int CurrentPosition { get; set; }

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// The add.
        /// </summary>
        /// <returns>
        /// The <see cref="XElementInfo"/>.
        /// </returns>
        internal XElementInfo Add()
        {
            return this;
        }

        /// <summary>
        /// The create of.
        /// </summary>
        /// <param name="element">
        /// The element.
        /// </param>
        /// <returns>
        /// The <see cref="XElementInfo"/>.
        /// </returns>
        internal XElementInfo CreateOf(XElement element)
        {
            var info = this.CreateOf(new XElementInfo(element));

            this.ChildrenInfo = info.ChildrenInfo;
            this.Element = info.Element;
            this.Name = info.Name;
            this.Children = info.Children;
            return this;
        }

        /// <summary>
        /// The initial.
        /// </summary>
        /// <param name="element">
        /// The element.
        /// </param>
        /// <returns>
        /// The <see cref="XElementInfo"/>.
        /// </returns>
        private XElementInfo CreateOf(XElementInfo element)
        {
            var parent = element.Element;
            
            foreach (var child in parent.Elements())
            {
                if (child.HasElements)
                {
                    var elementInfo = new XElementInfo(child);
                    element.ChildrenInfo = elementInfo.CreateOf(elementInfo);
                }

                element.Children.Add(child);
            }

            return element;
        }
    }
}
