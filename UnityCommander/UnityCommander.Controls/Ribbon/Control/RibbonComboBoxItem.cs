
namespace UnityCommander.Controls.Ribbon.Control
{
    using System;
    using System.Windows.Input;
    using UnityCommander.Common.Commands;
    using UnityCommander.Common.Models.Icons;

    /// <summary>
    /// The ribbon combo box.
    /// </summary>
    public class RibbonComboBoxItem : RibbonControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RibbonComboBoxItem"/> class.
        /// </summary>
        /// <param name="content">
        /// The content.
        /// </param>
        /// <param name="buttonIcon">
        /// The button icon.
        /// </param>
        /// <param name="buttonCommand">
        /// The button command.
        /// </param>
        public RibbonComboBoxItem(string content, IIcon buttonIcon, RibbonCommand buttonCommand)
            : base(
                content, 
                buttonIcon, 
                buttonCommand, 
                "RibbonComboBoxItemStyles", 
                "RibbonComboBoxItemTemplate", 
                "RibbonComboBoxItemDataTemplate")
        {
        }
    }
}
