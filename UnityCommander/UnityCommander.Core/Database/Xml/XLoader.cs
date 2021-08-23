
namespace UnityCommander.Core.Database.Xml
{
    using System.Collections.Generic;

    /// <summary>
    /// The document type.
    /// </summary>
    public enum DocumentType
    {
        /// <summary>
        /// The app session.
        /// </summary>
        AppSession,

        /// <summary>
        /// The app settings.
        /// </summary>
        AppSettings
    }

    /// <summary>
    /// The xml loader.
    /// </summary>
    public class XDocumentLoader
    {
        /// <summary>
        /// The documents.
        /// </summary>
        private readonly List<XDocument> documents = new ();

        /// <summary>
        /// Initializes a new instance of the <see cref="XDocumentLoader"/> class.
        /// </summary>
        public XDocumentLoader()
        {
            XDocument appSession = new XDocument("appSession", DocumentType.AppSession.ToString());
            this.documents.Add(appSession);
        }

        /// <summary>
        /// The get file panel state.
        /// </summary>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public object GetFilePanelState()
        {
            return null;
        }

        /// <summary>
        /// The save file panel state.
        /// </summary>
        public void SaveFilePanelState()
        {
        }
    }
}
