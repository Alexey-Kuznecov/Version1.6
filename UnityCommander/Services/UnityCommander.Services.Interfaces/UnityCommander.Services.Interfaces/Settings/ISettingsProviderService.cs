namespace UnityCommander.Services.Interfaces.Settings
{
    using Config.Net;

    /// <summary>
    /// The ProviderSettings interface.
    /// </summary>
    public interface ISettingsProviderService
    {
        /// <summary>
        /// The get app configuration.
        /// </summary>
        /// <returns>
        /// The <see cref="ISettings"/>.
        /// </returns>
        ISettings GetAppConfig();
    }
}
