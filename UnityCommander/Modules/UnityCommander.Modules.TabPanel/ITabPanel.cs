
namespace UnityCommander.Modules.TabPanel
{
    using UnityCommander.Common.Module;

    /// <summary>
    /// The PanelContainer interface.
    /// </summary>
    public interface ITabPanel
    {
        /// <summary>
        /// Gets or sets the current region name.
        /// </summary>
        public string CurrentRegionName { get; set; }

        /// <summary>
        /// Initial directory panel.
        /// </summary>
        /// <param name="regionName">
        /// The region name.
        /// </param>
        public void InitialTabPanelContent(string regionName);

        /// <summary>
        /// The set current tab panel.
        /// </summary>
        /// <param name="tabPanel">
        /// The tab panel.
        /// </param>
        public void SetCurrentTabPanel(object tabPanel);

        /// <summary>
        /// The set active tab panel content.
        /// </summary>
        /// <param name="tabContent">
        /// The tab content.
        /// </param>
        public void SetActiveTabPanelContent(ITabPanelContent tabContent);
    } 
}
