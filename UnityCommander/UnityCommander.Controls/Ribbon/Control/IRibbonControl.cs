using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace UnityCommander.Controls.Ribbon.Control
{
    public interface IRibbonControl
    {
        /// <summary>
        /// Gets or sets the buttonTemplate.
        /// </summary>
        public ControlTemplate Template { get; set; }

        /// <summary>
        /// Gets or sets the buttonTemplate.
        /// </summary>
        public DataTemplate DataTemplate { get; set; }

        /// <summary>
        /// Gets or sets the buttonStyle.
        /// </summary>
        public Style Style { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        public RibbonControlModel DataContext { get; set; }
    }
}
