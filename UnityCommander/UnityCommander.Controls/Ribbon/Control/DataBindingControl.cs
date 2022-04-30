
namespace UnityCommander.Controls.Ribbon.Control
{
    using System.Windows.Input;
    using System.Windows.Shapes;
    using UnityCommander.Common.Commands;

    /// <summary>
    /// The type of data model to bind data to the tool ribbon control.
    /// </summary>
    public class DataBindingControl
    {
        /// <summary>
        /// Gets or sets the content for the control.
        /// </summary>
        public object Content { get;set; }

        /// <summary>
        /// Gets or sets the icon for the control.
        /// </summary>
        public Path Icon { get; set; }

        /// <summary>
        /// Gets or sets the command for the control.
        /// </summary>
        public IGlobalCommand GlobalCommand { get; set; }
    }
}
