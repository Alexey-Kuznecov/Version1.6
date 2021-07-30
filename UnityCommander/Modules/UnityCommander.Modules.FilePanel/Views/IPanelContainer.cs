
namespace UnityCommander.Modules.FilePanel.Views
{
    using System;

    /// <summary>
    /// The PanelContainer interface.
    /// </summary>
    public interface IPanelContainer
    {
        /// <summary>
        /// Gets or sets the initial command.
        /// </summary>
        /// <param name="panelToken">
        /// The command manager.
        /// </param>
        public void InitialPanel(Guid[] panelToken);
    } 
}
