
namespace UnityCommander.Services.Interfaces.Database.Queries.Xml
{
    using System;
    using System.IO;
    using System.Xml.Linq;

    /// <summary>
    /// The x document.
    /// </summary>
    public class XDocument
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XDocument"/> class.
        /// </summary>
        /// <param name="fileName">
        /// The path.
        /// </param>
        /// <param name="documentType">
        /// The document type.
        /// </param>
        public XDocument(string fileName, string documentType)
        {
            var directoryPath = AppDomain.CurrentDomain.BaseDirectory;
            this.Path = System.IO.Path.Combine(directoryPath ?? string.Empty, fileName + ".xml");

            if (File.Exists(this.Path))
            {
                var root = XElement.Load(this.Path);
                this.ElementInfo = new XElementInfo();
                this.ElementInfo.CreateOf(root);
            }
            else
            {
                this.Root = new XElement(documentType);
            }
        }

        /// <summary>
        /// Gets or sets the path to the xml document.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets the rootName.
        /// </summary>
        public XElement Root { get; set; }

        /// <summary>
        /// Gets the list.
        /// </summary>
        public XElementInfo ElementInfo { get; }

        /// <summary>
        /// The initial.
        /// </summary>
        /// <param name="element">
        /// The element.
        /// </param>
        private void Save(XElement element)
        {
        }
    }
}
