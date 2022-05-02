
namespace UnityCommander.Modules.TabPanel
{
    using UnityCommander.Common.Module;
    using UnityCommander.Modules.TabPanel.Behaviors;

    /// <summary>
    /// The PanelContainer interface.
    /// </summary>
    public interface ITabPanel
    {
        /// <summary>
        /// Gets or sets the current region name.
        /// </summary>
        public string RegionContentName { get; set; }

        /// <summary>
        /// Initial directory panel.
        /// </summary>
        /// <param name="regionName">
        /// The region name.
        /// </param>
        public void InitialTabPanelContent(string regionName);
    } 
}
