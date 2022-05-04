
namespace UnityCommander.Integration.Options
{
    using UnityCommander.Integration.Contracts;

    /// <summary>
    /// The Settings interface.
    /// </summary>
    public interface IPluginSettings : IPluginService
    {
        /// <summary>
        /// The on settings changed.
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        public void OnSettingsChanged(SettingsBase settings);
    }
}
