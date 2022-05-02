
namespace UnityCommander.Controls.Ribbon
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    using AlexeyKuznecov.Library.Mvvm.Base;

    /// <summary>
    /// The ribbon.
    /// </summary>
    public class Ribbon : Panel
    {
        /// <summary>
        /// The ribbon initial items.
        /// </summary>
        public static readonly DependencyProperty RibbonManagerProperty =
            DependencyProperty.Register(
                "RibbonManager",
                typeof(IRibbonManager), 
                typeof(Ribbon), 
                new PropertyMetadata(OnRibbonManagerChangedCallback));

        /// <summary>
        /// The ribbon width.
        /// </summary>
        private static double ribbonWidth;

        /// <summary>
        /// The ribbon.
        /// </summary>
        private static Ribbon ribbon;

        /// <summary>
        /// Gets or sets the tab command.
        /// </summary>
        public static ICommand MinimizeCommand = new RelayCommand(
            obj =>
                {
                    var parent = ribbon.Parent as FrameworkElement;

                    while (parent?.Name != "RibbonExpandButtonHere")
                    {
                        parent = parent?.Parent as FrameworkElement;

                        if (parent?.Name == "CollapseHere")
                        {
                            parent.Visibility = parent.IsVisible ? Visibility.Hidden : Visibility.Visible;
                        }
                    }
                });

        /// <summary>
        /// Initializes a new instance of the <see cref="Ribbon"/> class.
        /// </summary>
        public Ribbon()
        {
        }

        /// <summary>
        /// Gets or sets the ribbon initial items property.
        /// </summary>
        public IRibbonManager RibbonManager
        {
            get => (IRibbonManager)this.GetValue(RibbonManagerProperty);
            set => this.SetValue(RibbonManagerProperty, value);
        }

        /// <summary>
        /// The on ribbon manager changed callback.
        /// </summary>
        /// <param name="d">
        /// The d.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void OnRibbonManagerChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var manager = e.NewValue as IRibbonManager;
            ribbon = (Ribbon)d;
            manager?.Collapse(MinimizeCommand);
            manager?.Initial(ribbon);
        }

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

            while (parent is not Window)
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
