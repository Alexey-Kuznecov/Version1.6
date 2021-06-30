using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace AIconBrowser.Database.Xml
{
    /// <summary>
    /// The xml parser.
    /// </summary>
    public abstract class XmlParser
    {
        #region Constructors

        private int _posParser;

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlParser"/> class.
        /// </summary>
        protected XmlParser() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlParser"/> class.
        /// </summary>
        /// <param name="path"> Path to the xml document. </param>
        public XmlParser(string path)
        {
            this.Path = path;
            this.DocumentRoot = XElement.Load(path);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlParser"/> class.
        /// </summary>
        /// <param name="path"> Path to the xml document. </param>
        /// <param name="nestLevel"> Nesting level of the xml elements. </param>
        protected XmlParser(string path, byte nestLevel)
        {
            this.NestLevel = nestLevel;
            this.Path = path;
            this.DocumentRoot = XElement.Load(path);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the position of the current item.
        /// </summary>
        public int CurrentElementPosition { get; set; }

        /// <summary>
        /// Gets or sets the position of the parser.
        /// </summary>
        public int ParserPosition
        {
            get => this._posParser;
            set
            {
                this._posParser = value;
            }
        }

        /// <summary>
        /// Gets the nesting level of the xml elements.
        /// </summary>
        private byte NestLevel { get; }

        /// <summary>
        /// Gets or sets the current node at the parser position.
        /// </summary>
        protected XElement CurrentNode { get; set; }

        /// <summary>
        /// Gets the path to the xml document.
        /// </summary>
        protected string Path { get; }

        /// <summary>
        /// Gets the root element of an xml document.
        /// </summary>
        protected XElement DocumentRoot { get; }

        /// <summary>
        /// Gets or sets a list of xml elements that contain child elements.
        /// </summary>
        private List<XElement> HasElementsList { get; set; }

        #endregion

        #region Methods

        public void AddNewElement(XElement newElement)
        {
            this.HasElementsList.Add(newElement);
        }

        /// <summary>
        /// The method applies an <paramref name="action"/> to each element of the xml document.
        /// The action is passed as method parameters. This method works in tandem with the <see name="AddHasElement(action)"/> method.
        /// </summary>
        /// <param name="action"> The action that applies to each item. </param>
        public virtual void ApplyForEach(Action<XElement> action)
        {
            this.AddHasElement();

            while (this.HasElementsList.Count != 0)
            {
                foreach (var item in this.HasElementsList.Elements())
                {
                    action(item);

                    if (item.HasElements)
                    {
                        this.AddHasElement(item, action);
                    }

                    if (item.NextNode == null)
                    {
                        this.CurrentElementPosition = 0;

                        if (item.Parent == null)
                        {
                            this.HasElementsList.RemoveAt(ParserPosition);
                        }
                        else
                        {
                            this.HasElementsList.Remove(item.Parent);
                        }

                        break;
                    }

                    this.CurrentElementPosition++;
                }

                this.ParserPosition++;
            }

            this.SaveDocument();
        }

        /// <summary>
        /// The method makes changed in the xml document.
        /// </summary>
        protected virtual void SaveDocument()
        {
            this.DocumentRoot.Save(Path);
        }

        /// <summary>
        /// The method fills the <see cref="HasElementsList"/> collection of the top-level xml elements.
        /// </summary>
        protected void AddHasElement()
        {
            this.HasElementsList = new List<XElement>();

            foreach (var item in this.CurrentNode.Elements())
            {
                if (item.HasElements)
                {
                    this.HasElementsList.Add(item);
                }
            }
        }

        /// <summary>
        /// The method adds elements that has a children elements 
        /// in the <see cref="HasElementsList"/> collection. 
        /// </summary>
        /// <param name="parent"> An element that contains child elements. </param>
        /// <param name="action"> The action that applies to each item. </param>
        protected void AddHasElement(XElement parent, Action<XElement> action)
        {
            foreach (var item in parent.Elements())
            {
                action(item);

                if (item.HasElements)
                {
                    this.HasElementsList.Add(item);
                }
            }
        }

        /// <summary>
        /// The method finds an element with a specific attribute of the xml document.
        /// </summary>
        /// <param name="specificAttribute">
        /// The specific attribute indicates the start of reading the document.
        /// </param>
        protected void GetCurrentElement(XAttribute specificAttribute)
        {
            var query = from element in XmlExtension.GetElements(this.DocumentRoot, NestLevel)
                        where element.Attribute(specificAttribute.Name)?.Value == specificAttribute.Value
                        where element.Attribute(specificAttribute.Name)?.Name == specificAttribute.Name
                        select element;
            this.CurrentNode = query.Single();
        }

        /// <summary>
        /// The method finds an specific element of the xml document.
        /// </summary>
        /// <param name="specificElement">
        /// The specific element indicates the start of reading the document.
        /// </param>
        protected void GetCurrentElement(XElement specificElement)
        {
            var query = from element in XmlExtension.GetElements(this.DocumentRoot, NestLevel)
                        where element.Name == specificElement.Name 
                        select element;

            this.CurrentNode = new XElement("Container", query);
        }

        #endregion
    }
}
