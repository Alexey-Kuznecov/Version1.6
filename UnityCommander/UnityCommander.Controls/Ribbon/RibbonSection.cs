
namespace UnityCommander.Controls.Ribbon
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    /// <summary>
    /// The ribbon item.
    /// </summary>
    public class RibbonSection : Panel
    {
        /// <summary>
        /// TODO The margin section.
        /// </summary>
        private static double marginGroup;

        /// <summary>
        /// The on render.
        /// </summary>
        /// <param name="dc">
        /// The dc.
        /// </param>
        protected override void OnRender(DrawingContext dc)
        {
            SolidColorBrush mySolidColorBrush = new SolidColorBrush(Color.FromRgb(247, 246, 245));
            Pen myPen = new Pen(new SolidColorBrush(Color.FromArgb(255, 229, 228, 229)), 1);
            Rect myRect = new Rect(0, 1, 1920, 104);
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

            foreach (UIElement child in this.Children)
            {
                child.Arrange(new Rect(new Point(margin, 0), child.DesiredSize));
                margin += child.DesiredSize.Width;
            }

            return arrangeBounds;
        }

        /// <summary>
        /// The width adorner.
        /// </summary>
        private static double widthSection;

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

            Size size = new Size(double.PositiveInfinity, double.PositiveInfinity);
            foreach (UIElement child in this.Children)
            {
                child.Measure(size);
                width += child.DesiredSize.Width;
                height = child.DesiredSize.Height;
            }

            return new Size();
        }
    }
}
