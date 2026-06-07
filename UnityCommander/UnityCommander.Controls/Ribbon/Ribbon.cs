
namespace UnityCommander.Controls.Ribbon
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using UnityCommander.Mvvm;

    public class Ribbon : Panel
    {
        public static readonly DependencyProperty RibbonManagerProperty =
            DependencyProperty.Register(
                "RibbonManager",
                typeof(IRibbonManager), 
                typeof(Ribbon), 
                new PropertyMetadata(OnRibbonManagerChangedCallback));

        private static double ribbonWidth; 
        private static Ribbon ribbon;
        public static ICommand MinimizeCommand
            = new RelayCommand(
            obj =>
                {
                    HideRibbon();
                });
        private RibbonConfig ribbonConfig;
        public Ribbon()
        {
            this.ribbonConfig = new RibbonConfig();
        }

        public IRibbonManager RibbonManager
        {
            get => (IRibbonManager)this.GetValue(RibbonManagerProperty);
            set => this.SetValue(RibbonManagerProperty, value);
        }

        private static void OnRibbonManagerChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var manager = e.NewValue as IRibbonManager;
            ribbon = (Ribbon)d;
            manager?.Collapse(MinimizeCommand);
            manager?.Initial(ribbon);
            manager?.Configure(ribbon.ribbonConfig);
        }

        private static void HideRibbon()
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
        }

        #region Override methods

        protected override Size ArrangeOverride(Size finalSize)
        {
            double margin = 0;

            foreach (UIElement child in this.Children)
            {
                child.Arrange(new Rect(new Point(margin, 0), child.DesiredSize));
                margin += child.DesiredSize.Width;
            }

            if (!ribbon.ribbonConfig.Visibility)
                HideRibbon();

            return finalSize;
        }

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
