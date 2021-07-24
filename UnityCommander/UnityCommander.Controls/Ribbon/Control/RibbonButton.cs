
namespace UnityCommander.Controls.Ribbon.Control
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Shapes;
    using UnityCommander.Common.Models.Icons;

    /// <summary>
    /// The ribbon item model.
    /// </summary>
    public class RibbonButton
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
        public RibbonButton(IIcon buttonIcon, ICommand buttonCommand, ControlTemplate newButtonTemplate, Style newButtonStyle)
            : this(buttonIcon, buttonCommand)
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
        public RibbonButton(IIcon buttonIcon, ICommand buttonCommand)
        {
            this.DataContext = new RibbonControlModel { Command = buttonCommand, Icon = buttonIcon.GetIconPath() };
            this.Template = this.buttonTemplate ?? (ControlTemplate)Application.Current.TryFindResource("RibbonButtonTemplate");
            this.Style = this.buttonStyle ?? (Style)Application.Current.TryFindResource("RibbonButtonStyles");
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

        /// <summary>
        /// The ribbon control model.
        /// </summary>
        public class RibbonControlModel
        {
            /// <summary>
            /// Gets or sets the buttonIcon.
            /// </summary>
            public Path Icon { get; set; }

            /// <summary>
            /// Gets or sets the buttonCommand.
            /// </summary>
            public ICommand Command { get; set; }
        }
    }
}
