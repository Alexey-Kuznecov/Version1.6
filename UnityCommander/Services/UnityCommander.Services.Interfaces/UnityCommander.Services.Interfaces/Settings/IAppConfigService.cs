namespace UnityCommander.Services.Interfaces.Settings
{
    using UnityCommander.Services.Interfaces.Database.Queries.Xml;

    /// <summary>
    /// The AppConfigService interface.
    /// </summary>
    public interface IAppConfigService
    {
        /// <summary>
        /// The get app session.
        /// </summary>
        /// <returns>
        /// The <see cref="XDocument"/>.
        /// </returns>
        XDocument GetSession();
    }
}
