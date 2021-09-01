
namespace UnityCommander.Controls.Taber
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;

    using Point = System.Windows.Point;
    using Size = System.Windows.Size;

    /// <summary>
    /// The taber control.
    /// </summary>
    [SuppressMessage("ReSharper", "StyleCop.SA1503")]
    public class TabPanel : Panel
    {
        /// <summary>
        /// The my property property.
        /// </summary>
        public static readonly DependencyProperty CollectionProperty =
            DependencyProperty.Register("Collection", typeof(TabCollection), typeof(TabPanel), new PropertyMetadata(null, OnInitialElementsChangedCallback, CoerceValueCallback));

        /// <summary>
        /// Gets or sets the my property.
        /// </summary>
        public TabCollection Collection
        {
            get => (TabCollection)this.GetValue(CollectionProperty);
            set => this.SetValue(CollectionProperty, value);
        }

        /// <summary>
        /// The measure override.
        /// </summary>
        /// <param name="constraint">
        /// The constraint.
        /// </param>
        /// <returns>
        /// The <see cref="System.Windows.Size"/>.
        /// </returns>
        protected override Size MeasureOverride(Size constraint)
        {
            Size size = new Size(double.PositiveInfinity, double.PositiveInfinity);
            foreach (UIElement child in this.Children)
            {
                child.Measure(size);
            }

            return new Size();
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
        /// The initial elements changed callback.
        /// </summary>
        /// <param name="d">
        /// The d.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void OnInitialElementsChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TabPanel tabControl && tabControl.Collection != null)
            {
                if (tabControl.Collection.Count > 0)
                {
                    tabControl.Collection.CollectionChanged += tabControl.OnCollectionChanged;

                    foreach (Control control in tabControl.Collection)
                    {
                        tabControl.Children.Add(control);
                    }
                } 
            }
        }

        /// <summary>
        /// The coerce value callback.
        /// </summary>
        /// <param name="d">
        /// The d.
        /// </param>
        /// <param name="value">
        /// The base value.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        private static object CoerceValueCallback(DependencyObject d, object value)
        {
            return value;
        }

        /// <summary>
        /// The on collection changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void OnCollectionChanged(object sender, System.EventArgs e)
        {
            if (sender is TabCollection collection)
            {
                Control addControl = default(Control);
                this.Children.Clear();

                foreach (Control control in collection)
                {
                    if (control is AddTabControl)
                    {
                        addControl = control;
                        continue;
                    }

                    this.Children.Add(control);
                }

                this.Children.Add(addControl ?? throw new InvalidOperationException());
            }
        }
    }
}