using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UnityCommander.Common.Models.Icons;

namespace UnityCommander.Controls.Ribbon.Control
{
    public class RibbonListBox : IRibbonControl
    {
        /// <summary>
        /// The button template.
        /// </summary>
        private readonly ControlTemplate buttonTemplate;

        /// <summary>
        /// The button style.
        /// </summary>
        private readonly Style buttonStyle;

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
        public RibbonListBox(object content, IIcon buttonIcon, ICommand buttonCommand, ControlTemplate newButtonTemplate, Style newButtonStyle)
            : this(content, buttonIcon, buttonCommand)
        {
            this.buttonTemplate = newButtonTemplate;
            this.buttonStyle = newButtonStyle;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RibbonButton"/> class.
        /// </summary>
        /// <param name="buttonIcon">
        /// The button icon.
        /// </param>
        /// <param name="buttonCommand">
        /// The button command.
        /// </param>
        public RibbonListBox(object content, IIcon buttonIcon, ICommand buttonCommand)
        {
            this.DataContext = new RibbonControlModel { Content = content, Command = buttonCommand, Icon = buttonIcon.GetIconPath() };
            this.Template = this.buttonTemplate ?? (ControlTemplate)Application.Current.FindResource("RibbonListBoxItemTemplate");
            this.Style = this.buttonStyle ?? (Style)Application.Current.FindResource("RibbonListBoxItemStyles");
        }

        /// <summary>
        /// Gets or sets the buttonTemplate.
        /// </summary>
        public ControlTemplate Template { get; set; }

        /// <summary>
        /// Gets or sets the buttonStyle.
        /// </summary>
        public Style Style { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        public RibbonControlModel DataContext { get; set; }

        public DataTemplate DataTemplate { get; set; }
    }
}
