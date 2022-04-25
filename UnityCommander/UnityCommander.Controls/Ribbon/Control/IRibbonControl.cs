
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRibbonControl.cs" company="Alexei Kuznecov">
//  All rights reserved.
// </copyright>
// <summary>
//  This interface requires the necessary data to correctly add new controls for the Tool Ribbon.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace UnityCommander.Controls.Ribbon.Control
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// This interface requires the necessary data to correctly add new controls for the Tool Ribbon.
    /// </summary>
    public interface IRibbonControl
    {
        /// <summary>
        /// Gets or sets the template of a control such as Button.
        /// </summary>
        public ControlTemplate Template { get; set; }

        /// <summary>
        /// Gets or sets the data model type. Use this property to bind data in a control template.
        /// </summary>
        public DataBindingControl DataBinding { get; set; }

        /// <summary>
        /// Gets or sets the data template for elements of the enumerated type
        /// </summary>
        public DataTemplate DataTemplate { get; set; }

        /// <summary>
        /// Gets or sets styles for the control.
        /// </summary>
        public Style Style { get; set; }
    }
}
