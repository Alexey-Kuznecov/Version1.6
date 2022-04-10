using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UnityCommander.Common.Models.Icons;

namespace UnityCommander.Controls.Ribbon.Control
{
    public class RibbonControl : IRibbonControl
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
        public RibbonControl(
            object content,
            IIcon buttonIcon,
            ICommand buttonCommand,
            string styleName,
            string templateName,
            string dataTemplate)
        {
            this.DataContext = new RibbonControlModel 
            {
                Content = content,
                Command = buttonCommand,
                Icon = buttonIcon.GetIconPath()
            };

            this.Template = (ControlTemplate)Application.Current.FindResource(templateName);
            this.Style = (Style)Application.Current.FindResource(styleName);
            this.DataTemplate = (DataTemplate)Application.Current.FindResource(dataTemplate);
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
