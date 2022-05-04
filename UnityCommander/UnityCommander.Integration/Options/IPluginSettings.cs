
namespace UnityCommander.Integration.Options
{
    /// <summary>
    /// The Settings interface.
    /// </summary>
    public interface IPluginSettings
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
