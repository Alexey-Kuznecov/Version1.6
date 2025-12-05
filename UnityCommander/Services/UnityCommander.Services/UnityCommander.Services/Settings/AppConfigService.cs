
namespace UnityCommander.Services.Settings
{
    using UnityCommander.Services.Interfaces.Database.Queries.Xml;
    using UnityCommander.Services.Interfaces.Settings;

    /// <summary>
    /// The app config service.
    /// </summary>
    public class AppConfigService : IAppConfigService
    {
        /// <summary>
        /// The app session.
        /// </summary>
        private readonly XDocument appSession;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="AppConfigService"/> class.
        /// </summary>
        public AppConfigService()
        {
            appSession = new XDocument("app.session", "AppSession");
        }

        /// <summary>
        /// The get app session.
        /// </summary>
        /// <returns>
        /// The <see cref="XDocument"/>.
        /// </returns>
        public XDocument GetSession() => appSession;
    }
}
