
namespace UnityCommander.Controls.Ribbon
{
    using System.Windows.Controls;

    using UnityCommander.Controls.Ribbon.Control;

    /// <summary>
    /// The ribbon item.
    /// </summary>
    public class RibbonElement : ContentControl
    {
        /// <summary>
        /// The content control.
        /// </summary>
        private readonly ContentControl contentControl = new ();

        /// <summary>
        /// Initializes a new instance of the <see cref="RibbonElement"/> class.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        public RibbonElement(IRibbonControl item)
        {
            this.contentControl.DataContext = item.DataBinding;
            this.contentControl.Template = item.Template;
            this.contentControl.Style = item.Style;
            this.Content = this.contentControl;
        }
    }
}
