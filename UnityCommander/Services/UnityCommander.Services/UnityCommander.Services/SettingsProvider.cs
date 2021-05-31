
namespace UnityCommander.Services
{
    using Config.Net;

    using UnityCommander.Services.Interfaces;

    /// <summary>
    /// The settings provider.
    /// </summary>
    public class SettingsProvider : ISettingsProvider
    {
        /// <summary>
        /// The get app settings.
        /// </summary>
        /// <returns>
        /// The <see cref="ISettings"/>.
        /// </returns>
        public ISettings GetAppConfig()
        {
            return new ConfigurationBuilder<ISettings>().UseAppConfig().Build();
        }
    }
}
