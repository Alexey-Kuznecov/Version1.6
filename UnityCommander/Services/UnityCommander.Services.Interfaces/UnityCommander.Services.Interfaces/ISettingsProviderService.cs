
namespace UnityCommander.Services.Interfaces
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
