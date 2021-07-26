
namespace UnityCommander.Controls.Ribbon
{
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using UnityCommander.Integration.Mvvm.Base;

    /// <summary>
    /// The ribbon tab.
    /// </summary>
    public class RibbonTab : Panel
    {
        private Size windowSize;

        public RibbonTab()
        {
            //var window = this.GetWindow();
            //if (window != null)
            //{
            //    window.SizeChanged += Window_SizeChanged;
            //}
        }

        /// <summary>
        /// Gets or sets the tab command.
        /// </summary>
        public ICommand MinimizeCommand => new RelayCommand(obj =>
        {
            FrameworkElement parent = this.Parent as FrameworkElement;

            while (!(parent is Ribbon))
            {
                parent = parent?.Parent as FrameworkElement;

                if (parent is Ribbon ribbon)
                {
                    foreach (var item in ribbon.Children)
                    {
                        if (item is Grid grid && grid.Children[1] is RibbonSection section)
                        {
                            section.Visibility = section.IsVisible ? Visibility.Hidden : Visibility.Visible;
                        }
                    }
                }
            }
        });

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
            
            foreach (UIElement child in this.Children)
            {
                child.Arrange(new Rect(new Point(margin, 0), child.DesiredSize));

                if (child is Grid tabContainer)
                {
                    foreach (UIElement item in tabContainer.Children)
                    {
                        if (item is Button button)
                        {
                            item.Arrange(new Rect(new Point(margin, 0), item.DesiredSize));
                            button.Style = (Style)Application.Current.FindResource("RibbonTabStyle");
                        }
                        if (item.GetType() == typeof(ContentControl))
                        {
                            var minimizeButton = (ContentControl)item;
                            minimizeButton.InputBindings.Add(new MouseBinding(this.MinimizeCommand, new MouseGesture(MouseAction.LeftClick)));
                            item.Arrange(new Rect(new Point(x: 1560, 1), item.DesiredSize));
                        }

                        margin += item.DesiredSize.Width + 2;
                    }
                }
            }

            return finalSize;
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var window = (Window)sender;
            this.windowSize = new Size(window.Width, window.Height);
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
