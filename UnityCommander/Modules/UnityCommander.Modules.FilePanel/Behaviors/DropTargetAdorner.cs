
namespace UnityCommander.Modules.FilePanel.Behaviors
{
    using System.Windows;
    using System.Windows.Documents;
    using System.Windows.Media;

    /// <summary>
    /// The drop target adorner.
    /// </summary>
    public class DropTargetAdorner : Adorner
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DropTargetAdorner"/> class.
        /// </summary>
        /// <param name="adornedElement">
        /// The adorned element.
        /// </param>
        public DropTargetAdorner(UIElement adornedElement)
            : this(adornedElement, (DropInfo)null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DropTargetAdorner"/> class.
        /// </summary>
        /// <param name="adornedElement">
        /// The adorned element.
        /// </param>
        /// <param name="dropInfo">
        /// The drop Info.
        /// </param>
        public DropTargetAdorner(UIElement adornedElement, DropInfo dropInfo)
            : base(adornedElement)
        {
            this.DropInfo = dropInfo;
        }

        /// <summary>
        /// Gets or sets the drop info.
        /// </summary>
        public DropInfo DropInfo { get; set; }

        /// <summary>
        /// Gets or sets the pen which can be used for the render process.
        /// </summary>
        public Pen Pen { get; set; } = new Pen(Brushes.Gray, 2);
    }
}
