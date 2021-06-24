
namespace UnityCommander.Controls.Ribbon
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    /// <summary>
    /// The ribbon item.
    /// </summary>
    public class RibbonContainer : Panel
    {
        /// <summary>
        /// Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MyPropertyProperty =
            DependencyProperty.Register("MyProperty", typeof(int), typeof(Ribbon), new PropertyMetadata(0));

        /// <summary>
        /// Gets or sets the my property.
        /// </summary>
        public int MyProperty
        {
            get => (int)this.GetValue(MyPropertyProperty);
            set => this.SetValue(MyPropertyProperty, value);
        }

        /// <summary>
        /// The on render.
        /// </summary>
        /// <param name="dc">
        /// The dc.
        /// </param>
        protected override void OnRender(DrawingContext dc)
        {
            SolidColorBrush mySolidColorBrush = new SolidColorBrush(Color.FromRgb(237, 237, 237));
            Pen myPen = new Pen(new SolidColorBrush(Color.FromArgb(0, 255, 85, 255)), 1);
            Rect myRect = new Rect(0, 0, 1950, 120);
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

            for (var index = 0; index < this.InternalChildren.Count; index++)
            {
                UIElement child = this.InternalChildren[index];
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
            Size size = new Size(double.PositiveInfinity, double.PositiveInfinity);
            foreach (UIElement child in this.InternalChildren)
            {
                child.Measure(size);
            }

            return new Size();
        }
    }
}
