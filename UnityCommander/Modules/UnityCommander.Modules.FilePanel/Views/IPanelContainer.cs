
namespace UnityCommander.Modules.FilePanel.Views
{
    using UnityCommander.Core.Modules;

    /// <summary>
    /// The PanelContainer interface.
    /// </summary>
    public interface IPanelContainer
    {
        /// <summary>
        /// Initial directory panel.
        /// </summary>
        /// <param name="directoryPanel">
        /// The directory panel.
        /// </param>
        /// <param name="regionName">
        /// The region name.
        /// </param>
        public void InitialDirectoryPanel(IDirectoryPanel directoryPanel, string regionName);
    } 
}
