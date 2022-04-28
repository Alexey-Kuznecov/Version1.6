namespace UnityCommander.Common.Commands
{
    using System.Windows;
    using System.Windows.Controls;

    public class XArray : Panel
    {
        #region IsSortcutEnabledProperty

        /// <summary>
        /// Gets or Sets whether the control can be used as drag source.
        /// </summary>
        public static readonly DependencyProperty TypeProperty
            = DependencyProperty.RegisterAttached("Type",
                                                  typeof(object),
                                                  typeof(XArray),
                                                  new UIPropertyMetadata(false, TypeChanged));

        /// <summary>
        /// Gets whether the control can be used as drag source.
        /// </summary>
        public static object GetType(UIElement target)
        {
            return (object)target.GetValue(TypeProperty);
        }

        /// <summary>
        /// Sets whether the control can be used as drag source.
        /// </summary>
        public static void SetType(UIElement target, object value)
        {
            target.SetValue(TypeProperty, value);
        }

        private static void TypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is XArray { Children: { } } ch)
            {
                foreach (Control control in ch.Children)
                {
                }
            }
        }

        #endregion

        #region IsSortcutEnabledProperty

        /// <summary>
        /// Gets or Sets whether the control can be used as drag source.
        /// </summary>
        public static readonly DependencyProperty ContentProperty
            = DependencyProperty.RegisterAttached("Content",
                                                  typeof(object),
                                                  typeof(XArray),
                                                  new UIPropertyMetadata(false, ContentChanged));

        /// <summary>
        /// Gets whether the control can be used as drag source.
        /// </summary>
        public static object GetContent(UIElement target)
        {
            return (object)target.GetValue(TypeProperty);
        }

        /// <summary>
        /// Sets whether the control can be used as drag source.
        /// </summary>
        public static void SetContent(UIElement target, object value)
        {
            target.SetValue(TypeProperty, value);
        }

        private static void ContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is XArray { Children: { } } ch)
            {
            }
        }

        #endregion

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
            foreach (UIElement child in this.Children)
            {
                child.Arrange(new Rect(new Point(0, 0), child.DesiredSize));
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
            foreach (UIElement child in this.Children)
            {
                child.Measure(size);
            }

            return new Size();
        }

        #endregion
    }
}
