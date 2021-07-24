
namespace UnityCommander.Controls.Ribbon
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    /// <summary>
    /// The ribbon section.
    /// </summary>
    public class RibbonGroup : Panel
    {
        /// <summary>
        /// The container group width.
        /// </summary>
        private Size containerGroupWidth;

        /// <summary>
        /// The on render.
        /// </summary>
        /// <param name="dc">
        /// The dc.
        /// </param>
        protected override void OnRender(DrawingContext dc)
        {
            SolidColorBrush mySolidColorBrush = new SolidColorBrush(Color.FromRgb(247, 246, 245));
            Pen myPen = new Pen(new SolidColorBrush(Color.FromArgb(255, 229, 228, 229)), 2);
            Rect myRect = new Rect(0, 1, this.containerGroupWidth.Width, this.containerGroupWidth.Height);
            dc.DrawRectangle(mySolidColorBrush, myPen, myRect);
        }

        /// <summary>
        /// The arrange override.
        /// </summary>
        /// <param name="arrangeBounds">
        /// The arrange bounds.
        /// </param>
        /// <returns>
        /// The <see cref="Size"/>.
        /// </returns>
        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            double margin = 0;

            for (var index = 0; index < this.Children.Count; index++)
            {
                UIElement child = this.Children[index];
                child.Arrange(new Rect(new Point(margin, 0), child.DesiredSize));
                margin += child.DesiredSize.Width;
            }
            
            return arrangeBounds;
        }

        /// <summary>
        /// The measure override.
        /// </summary>
        /// <param name="availableSize">
        /// The available size.
        /// </param>
        /// <returns>
        /// The <see cref="Size"/>.
        /// </returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            double margin = 0;

            Size size = new Size(50, 50);
            foreach (UIElement child in this.Children)
            {
                child.Measure(size);
                margin += 50;
            }

            this.containerGroupWidth = new Size(margin + 2, 100);
            return this.containerGroupWidth;
        }
    }
}
