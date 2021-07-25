
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
            SolidColorBrush mySolidColorBrush = new SolidColorBrush(Color.FromRgb(243, 243, 243));
            Pen myPen = new Pen(new SolidColorBrush(Color.FromArgb(255, 229, 228, 229)), 2);
            Rect myRect = new Rect(0, 1, this.containerGroupWidth.Width, 104);
            dc.DrawRectangle(mySolidColorBrush, myPen, myRect);
        }

        private static double margin;

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
            margin = 0;

            foreach (UIElement child in this.Children)
            {
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
            double width = 0;
            double height = 0;

            Size size = new Size(50, 50);
            foreach (UIElement child in this.Children)
            {
                child.Measure(size);
                height = child.DesiredSize.Height;
                width += child.DesiredSize.Width;
            }

            this.containerGroupWidth = new Size(width + 2, height);
            return this.containerGroupWidth;
        }
    }
}
