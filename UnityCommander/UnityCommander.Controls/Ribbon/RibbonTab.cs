
namespace UnityCommander.Controls.Ribbon
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    /// <summary>
    /// The ribbon tab.
    /// </summary>
    public class RibbonTab : Panel
    {
        /// <summary>
        /// The on render.
        /// </summary>
        /// <param name="dc">
        /// The dc.
        /// </param>
        protected override void OnRender(DrawingContext dc)
        {
            //SolidColorBrush mySolidColorBrush = new SolidColorBrush(Color.FromArgb(0, 255, 255, 255));
            //Pen myPen = new Pen(new SolidColorBrush(Color.FromRgb(200, 85, 255)), 0);
            //Rect myRect = new Rect(0, 0, double.MaxValue, 25);
            //dc.DrawRectangle(mySolidColorBrush, myPen, myRect);
        }

        /// <summary>
        /// The arrange override.
        /// </summary>
        /// <param name="finalSize">
        /// The arrange bounds.
        /// </param>
        /// <returns>
        /// The <see cref="Size"/>.
        /// </returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            double margin = 0;

            for (var index = 0; index < this.Children.Count; index++)
            {
                UIElement child = this.Children[index];
                child.Arrange(new Rect(new Point(margin, 0), child.DesiredSize));
                margin += child.DesiredSize.Width + 2;

                if (child is Button bt)
                {
                    bt.Style = (Style)Application.Current.FindResource("RibbonTabStyle");
                }
            }

            return finalSize;
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
            Size size = new Size(double.PositiveInfinity, double.PositiveInfinity);

            // In our example, we just have one child. 
            // Report that our panel requires just the size of its only child.
            foreach (UIElement child in this.Children)
            {
                child.Measure(size);
            }

            return new Size();
        }
    }
}
