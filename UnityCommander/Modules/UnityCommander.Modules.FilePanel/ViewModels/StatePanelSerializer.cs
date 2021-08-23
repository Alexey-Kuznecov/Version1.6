
namespace UnityCommander.Modules.FilePanel.ViewModels
{
    using UnityCommander.Core.Database.Xml;

    /// <summary>
    /// The state panel serializer.
    /// </summary>
    public static class StatePanelSerializer
    {
        /// <summary>
        /// The xml serializer.
        /// </summary>
        /// <param name="o">
        /// The o.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool XmlSerialize(object o)
        {
            XDocument document = new XDocument("app.session", "AppSession");
            document.SerializeObject("FilePanel", o);
            return false;
        }

        /// <summary>
        /// The xml deserialize.
        /// </summary>
        /// <param name="o">
        /// The o.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool XmlDeserialize(object o)
        {
            XDocument document = new XDocument("app.session", "AppSession");
            document.SerializeObject("FilePanel", o);
            return false;
        }
    }
}
