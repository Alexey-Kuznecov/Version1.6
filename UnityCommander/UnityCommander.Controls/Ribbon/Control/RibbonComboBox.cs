using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UnityCommander.Common.Models.Icons;

namespace UnityCommander.Controls.Ribbon.Control
{
    public class RibbonComboBox : RibbonControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RibbonButton"/> class.
        /// </summary>
        /// <param name="buttonIcon">
        /// The button icon.
        /// </param>
        /// <param name="buttonCommand">
        /// The button command.
        /// </param>
        /// <param name="newButtonTemplate">
        /// The button template.
        /// </param>
        /// <param name="newButtonStyle">
        /// The button style.
        /// </param>
        public RibbonComboBox(object content, IIcon buttonIcon, ICommand buttonCommand)
            : base(content, buttonIcon, buttonCommand, "RibbonComboBoxItemStyles", "RibbonComboBoxItemTemplate", "RibbonComboBoxItemDataTemplate")
        {
        }
    }
}
