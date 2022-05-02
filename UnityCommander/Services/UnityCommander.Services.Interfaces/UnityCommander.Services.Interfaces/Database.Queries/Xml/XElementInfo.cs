
namespace UnityCommander.Services.Interfaces.Database.Queries.Xml
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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

            if (element.LastAttribute == null) return;

            this.Attributes = element.Attributes().ToList();

            //foreach (var attribute in element.Attributes())
            //{
            //    if (attribute.Name != null)
            //    {
            //        this.Attributes.Add(element.FirstAttribute);
            //    }
            //}
                

            //if (element.LastAttribute != null)
            //{
            //    if (!this.Attributes.Contains(element.LastAttribute))
            //    {
            //        this.Attributes.Add(element.LastAttribute);
            //    }
            //}
        }

        #region Properties

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the element.
        /// </summary>
        public XElement Element { get; set; }

        /// <summary>
        /// Gets or sets the attributes.
        /// </summary>
        public List<XAttribute> Attributes { get; set; } = new ();

        /// <summary>
        /// Gets the has elements list.
        /// </summary>
        public List<XElement> Children { get; private set; } = new ();

        /// <summary>
        /// Gets the children info.
        /// </summary>
        public XElementInfo ChildrenInfo { get; private set; }

        /// <summary>
        /// Gets the children info.
        /// </summary>
        public XElementInfo ParentInfo { get; private set; }

        /// <summary>
        /// Gets the children info.
        /// </summary>
        public List<XElementInfo> ChildrenInfos { get; private set; } = new ();

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

        #endregion

        /// <summary>
        /// The find ancestor.
        /// </summary>
        /// <param name="element">
        /// The element.
        /// </param>
        /// <param name="level">
        /// The level.
        /// </param>
        /// <returns>
        /// The <see cref="XElementInfo"/>.
        /// </returns>
        public XElementInfo FindAncestor(XElementInfo element, byte level)
        {
            var counter = 0;
            var parent = element;

            while (counter < level)
            {
                parent = parent.ParentInfo;
                counter++;
            }

            return parent;
        }

        /// <summary>
        /// The get attribute by name.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetAttributeValueByName(string name)
        {
            foreach (var attribute in this.Attributes)
            {
                if (attribute.Name == name)
                {
                    return attribute.Value;
                }
            }

            return null;
        }

        /// <summary>
        /// The set attribute value by name.
        /// </summary>
        /// <param name="attributeName">
        /// The name.
        /// </param>
        /// <param name="newValue">
        /// The new Value.
        /// </param>
        public void SetAttributeValueByName(string attributeName, object newValue)
        {
            foreach (var attribute in this.Attributes)
            {
                if (attribute.Name == attributeName)
                {
                    attribute.Value = newValue.ToString() ?? string.Empty;
                    return;
                }
            }
        }

        /// <summary>
        /// The add.
        /// </summary>
        /// <param name="action">
        /// The action.
        /// </param>
        /// <returns>
        /// The <see cref="XElementInfo"/>.
        /// </returns>
        public XElementInfo Add(Func<XElementRecord, XElementRecord> action)
        {
            var result = action(new XElementRecord());
            var element = new XElement(result.Tag);
            this.Element.Add(element);

            if (result.Attributes.Count > 0)
            {
                foreach (var attribute in result.Attributes)
                {
                    element.SetAttributeValue(attribute.Key, attribute.Value);
                }
            }

            return this;
        }

        /// <summary>
        /// The add.
        /// </summary>
        /// <returns>
        /// The <see cref="XElementInfo"/>.
        /// </returns>
        public XElementInfo RemoveAll()
        {
            this.Element.RemoveAll();
            return this;
        }

        /// <summary>
        /// The find of.
        /// </summary>
        /// <param name="element">
        /// The element.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The <see cref="XElementInfo"/>.
        /// </returns>
        internal List<XElementInfo> FindOf(XElementInfo element, string name)
        {
            var items = new List<XElementInfo>();
            element.FindOf(element, name, ref items);
            return items;
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

            this.ParentInfo = info.ParentInfo;
            this.ChildrenInfo = info.ChildrenInfo;
            this.ChildrenInfos = info.ChildrenInfos;
            this.Element = info.Element;
            this.Name = info.Name;
            this.Children = info.Children;
            return this;
        }

        /// <summary>
        /// The find of.
        /// </summary>
        /// <param name="element">
        /// The element.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="results">
        /// The results.
        /// </param>
        /// <returns>
        /// The <see cref="XElementInfo"/>.
        /// </returns>
        private XElementInfo FindOf(XElementInfo element, string name, ref List<XElementInfo> results)
        {
            var parent = element;
            var result = default(XElementInfo);

            foreach (var child in parent.ChildrenInfos)
            {
                if (child != null)
                {
                    if (child.Name == name)
                    {
                        results.Add(child);
                    }
                    
                    result = child.FindOf(child, name, ref results);
                }
            }

            return result;
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
                var elementInfo = new XElementInfo(child);
                elementInfo.ParentInfo = element;
                element.ChildrenInfo = elementInfo.CreateOf(elementInfo);
                element.ChildrenInfos.Add(element.ChildrenInfo);
                element.Children.Add(child);
            }

            return element;
        }
    }
}
