
namespace UnityCommander.Controls.Ribbon.Control
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    using NJsonSchema.Annotations;

    using UnityCommander.Common.Models.Icons;

    /// <summary>
    /// All controls inherit the necessary functionality from this type.
    /// </summary>
    public class RibbonControl : IRibbonControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RibbonControl"/> class.
        /// </summary>
        /// <param name="controlName">
        /// The text of which will be displayed on the control.
        /// </param>
        /// <param name="controlIcon">
        /// The icon for the control.
        /// </param>
        /// <param name="controlCommand">
        /// The command for the control.
        /// </param>
        /// <param name="styleName">
        /// The style name for the control.
        /// </param>
        /// <param name="templateName">
        /// The name of the template for the control.
        /// </param>
        /// <param name="dataTemplate">
        /// Name of the data template for the control.
        /// </param>
        internal RibbonControl(
            [NotNull] string controlName,
            [NotNull] IIcon controlIcon,
            [NotNull] ICommand controlCommand,
            [NotNull] string styleName,
            [NotNull] string templateName,
            [CanBeNull] string dataTemplate)
        {
            this.DataBinding = new DataBindingControl 
            {
                Content = controlName,
                Command = controlCommand,
                Icon = controlIcon.GetIconPath()
            };

            this.Template = (ControlTemplate)Application.Current.FindResource(templateName);
            this.Style = (Style)Application.Current.FindResource(styleName);
            this.DataTemplate = (DataTemplate)Application.Current.TryFindResource(dataTemplate);
        }
        
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
