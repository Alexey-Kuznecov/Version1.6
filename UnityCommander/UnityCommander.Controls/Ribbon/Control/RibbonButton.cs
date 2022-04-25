
namespace UnityCommander.Controls.Ribbon.Control
{
    using System.Windows.Input;
    using UnityCommander.Common.Models.Icons;

    /// <summary>
    /// The data model to bind to the control.
    /// </summary>
    public class RibbonButton : RibbonControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RibbonButton"/> class.
        /// </summary>
        /// <param name="buttonName">
        /// The text of which will be displayed on the control.
        /// </param>
        /// <param name="buttonIcon">
        /// The icon for the control.
        /// </param>
        /// <param name="buttonCommand">
        /// The command for the control.
        /// </param>
        public RibbonButton(string buttonName, IIcon buttonIcon, ICommand buttonCommand)
            : base(
                buttonName, 
                buttonIcon, 
                buttonCommand, 
                "RibbonButtonStyles",
                "RibbonButtonTemplate",
                string.Empty)
        {
        }
    }
}
