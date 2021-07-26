
namespace UnityCommander.Controls.Ribbon
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// The ribbon.
    /// </summary>
    public class Ribbon : Panel
    {
        /// <summary>
        /// The ribbon width.
        /// </summary>
        private static double ribbonWidth;

        #region Override methods

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

            foreach (UIElement child in this.Children)
            {
                child.Arrange(new Rect(new Point(margin, 0), child.DesiredSize));
                margin += child.DesiredSize.Width;
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
            double height = 0;

            Size size = new Size(double.PositiveInfinity, double.PositiveInfinity);
            foreach (UIElement child in this.Children)
            {
                child.Measure(size);
                ribbonWidth += child.DesiredSize.Width;
                height = child.DesiredSize.Height;
            }

            return new Size(ribbonWidth, height);
        }

        #endregion

        /// <summary>
        /// The get window.
        /// </summary>
        /// <returns>
        /// The <see cref="Window"/>.
        /// </returns>
        private Window GetWindow()
        {
            FrameworkElement parent = this.Parent as FrameworkElement;

            while (!(parent is Window))
            {
                parent = parent?.Parent as FrameworkElement;

                if (parent is Window window)
                {
                    return window;
                }
            }

            return null;
        }
    }
}
