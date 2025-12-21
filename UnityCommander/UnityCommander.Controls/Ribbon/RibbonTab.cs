
namespace UnityCommander.Controls.Ribbon
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;

    using AlexeyKuznecov.Library.Mvvm.Base;

    public class RibbonTab : Panel
    {
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

        protected override void OnRender(DrawingContext dc)
        {
        }
     
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

                        margin += item.DesiredSize.Width + 2;
                    }
                }
            }

            return finalSize;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            Size size = new Size(double.PositiveInfinity, double.PositiveInfinity);

            foreach (UIElement child in this.Children)
            {
                child.Measure(size);
            }

            return new Size();
        }
    }
}
