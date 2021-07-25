
namespace UnityCommander.Controls.Ribbon
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    /// <summary>
    /// The group adorner.
    /// </summary>
    public class RibbonGroupAdorner : Grid
    {
        /// <summary>
        /// The set adorner.
        /// </summary>
        /// <param name="ribbonGroup">
        /// The ribbon Group.
        /// </param>
        /// <returns>
        /// The <see cref="RibbonGroupAdorner"/>.
        /// </returns>
        public RibbonGroupAdorner SetAdorner(RibbonGroup ribbonGroup)
        {
            this.Children.Add(ribbonGroup);
            return this;
        }

        /// <summary>
        /// The arrange override.
        /// </summary>
        /// <param name="arrangeSize">
        /// The arrange size.
        /// </param>
        /// <returns>
        /// The <see cref="Size"/>.
        /// </returns>
        protected override Size ArrangeOverride(Size arrangeSize)
        {
            double margin = 0;

            for (var index = 0; index < this.Children.Count; index++)
            {
                UIElement child = this.Children[index];
                child.Arrange(new Rect(new Point(margin, 0), child.DesiredSize));
            }

            return arrangeSize;
        }

        /// <summary>
        /// The get visual child.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="Visual"/>.
        /// </returns>
        protected override Visual GetVisualChild(int index)
        {
            return base.GetVisualChild(index);
        }

        /// <summary>
        /// The width adorner.
        /// </summary>
        private static double widthAdorner;

        /// <summary>
        /// The measure override.
        /// </summary>
        /// <param name="constraint">
        /// The constraint.
        /// </param>
        /// <returns>
        /// The <see cref="Size"/>.
        /// </returns>
        protected override Size MeasureOverride(Size constraint)
        {
            double width = 0;
            double height = 0;

            Size size = new Size(double.PositiveInfinity, double.PositiveInfinity);
            foreach (UIElement child in this.Children)
            {
                child.Measure(size);
                height = child.DesiredSize.Height;
                width += child.DesiredSize.Width;
            }

            return new Size(width, height);
        }
    }
}
