
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
            Rect myRect = new Rect(0, 1, 1920, 100);
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
                var ribbonGroup = child as RibbonGroup; 
                child.Arrange(new Rect(new Point(margin, 0), child.DesiredSize));
                
                if (ribbonGroup != null)
                {
                    margin += ribbonGroup.ActualWidth;
                }
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

            Size size = new Size(double.PositiveInfinity, double.PositiveInfinity);
            foreach (UIElement child in this.Children)
            {
                child.Measure(size);
                margin += child.DesiredSize.Width;
            }

            return new Size();
        }
    }
}
