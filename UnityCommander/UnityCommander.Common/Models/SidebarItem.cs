
namespace UnityCommander.Common.Models
{
    using System.Windows.Controls;

    using UnityCommander.Integration.Models;

    /// <summary>
    /// The sidebar item.
    /// </summary>
    public class SidebarItem
    {
        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        public UserControl Content { get; set; }

        /// <summary>
        /// Gets or sets the icon.
        /// </summary>
        public IconModel Icon { get; set; }
    }
}
