
namespace UnityCommander.Controls.Ribbon.Control
{
    using System.Windows.Input;

    using UnityCommander.Common.Models.Icons;
    using UnityCommander.Integration.Commands;

    /// <summary>
    /// The data model to bind to the control.
    /// </summary>
    public class RibbonListBoxItem : RibbonControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RibbonListBoxItem"/> class.
        /// </summary>
        /// <param name="listBoxItemName">
        /// The list box item name.
        /// </param>
        /// <param name="listBoxItemIcon">
        /// The list box item icon.
        /// </param>
        /// <param name="listBoxItemCommand">
        /// The list box item command.
        /// </param>
        public RibbonListBoxItem(string listBoxItemName, IIcon listBoxItemIcon, RibbonCommand listBoxItemCommand)
            : base(
                listBoxItemName,
                listBoxItemIcon,
                listBoxItemCommand,
                "RibbonListBoxItemStyles",
                "RibbonListBoxItemTemplate",
                string.Empty)
        {
        }
    }
}
