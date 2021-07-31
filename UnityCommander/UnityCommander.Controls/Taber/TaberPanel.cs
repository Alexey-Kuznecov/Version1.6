
namespace UnityCommander.Controls.Taber
{
    using System.Windows;
    using System.Windows.Controls;

    using Point = System.Windows.Point;
    using Size = System.Windows.Size;

    /// <summary>
    /// The taber control.
    /// </summary>
    public class TaberPanel : Panel
    {
        /// <summary>
        /// The my property property.
        /// </summary>
        public static readonly DependencyProperty InitialElementsProperty =
            DependencyProperty.Register("InitialElements", typeof(TabCollection), typeof(TaberPanel), new PropertyMetadata(null, OnInitialElementsChangedCallback, CoerceValueCallback));

        /// <summary>
        /// Gets or sets the my property.
        /// </summary>
        public TabCollection InitialElements
        {
            get => (TabCollection)this.GetValue(InitialElementsProperty);
            set => this.SetValue(InitialElementsProperty, value);
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

                if (child is TaberControl control)
                {
                }
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
            if (d is TaberPanel tabControl && tabControl.InitialElements != null)
            {
                if (tabControl.InitialElements.Count > 0)
                {
                    tabControl.InitialElements.CollectionChanged += tabControl.OnCollectionChanged;

                    var lastItem = default(TaberControl);

                    foreach (TaberControl element in tabControl.InitialElements)
                    {
                        if ((string)element.Content != "+")
                            tabControl.Children.Add(element);
                        else
                            lastItem = element;
                    }
                    lastItem.Width = 50;
                    tabControl.Children.Insert(tabControl.InitialElements.Count -1, lastItem);
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

        private void OnCollectionChanged(object sender, System.EventArgs e)
        {
            if (sender is TabCollection collectionChanged)
            {
                var lastItem = default(TaberControl);

                foreach (var item in collectionChanged)
                {
                    var control = (TaberControl)item;
                    if (!this.Children.Contains(control))
                    {
                        this.Children.Add(control);
                    }

                    if ((string)control.Content == "+")
                    {
                        lastItem = control;
                        this.Children.Remove(control);
                    }
                }

                this.Children.Insert(collectionChanged.Count - 1, lastItem);
            }
        }
    }
}