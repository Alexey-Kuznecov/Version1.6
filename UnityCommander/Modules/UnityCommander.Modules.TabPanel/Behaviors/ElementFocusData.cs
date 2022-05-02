
namespace UnityCommander.Modules.TabPanel.Behaviors
{
    using System.Windows;

    using UnityCommander.Common.Module;

    /// <summary>
    /// The element focus data.
    /// </summary>
    public class ElementFocusData
    {
        /// <summary>
        /// Gets or sets the element focusable.
        /// </summary>
        public FrameworkElement ElementFocusable { get; set; }

        /// <summary>
        /// Gets or sets the tab content.
        /// </summary>
        public ITabPanelContent TabContent { get; set; }

        /// <summary>
        /// Gets or sets the tab panel.
        /// </summary>
        public ITabPanel TabPanel { get; set; }
    }
}
