
namespace UnityCommander.Services
{
    using UnityCommander.Services.Interfaces;
    using UnityCommander.Services.Interfaces.Database.Queries.Xml;

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
            this.appSession = new XDocument("app.session", "AppSession");
        }

        /// <summary>
        /// The get app session.
        /// </summary>
        /// <returns>
        /// The <see cref="XDocument"/>.
        /// </returns>
        public XDocument GetSession() => this.appSession;
    }
}
