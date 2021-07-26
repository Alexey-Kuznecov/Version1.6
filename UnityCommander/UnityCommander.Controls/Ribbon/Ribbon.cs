
namespace UnityCommander.Controls.Ribbon
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;

    using AlexLibWpf.Mvvm.Base;

    /// <summary>
    /// The ribbon.
    /// </summary>
    [SuppressMessage("ReSharper", "StyleCop.SA1503")]
    public class Ribbon : Panel
    {
        #region Dependency fields

        /// <summary>
        /// Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty BubbleSourceProperty = DependencyProperty.Register(
            "BubbleSource",
            typeof(UserControl),
            typeof(Ribbon),
            new PropertyMetadata(null, OnBubbleSourceChanged, CoerceBubbleSource));

        /// <summary>
        /// Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SetCommandProperty = DependencyProperty.Register(
            "SetCommand",
            typeof(ICommand),
            typeof(Ribbon),
            new PropertyMetadata(new RelayCommand(() => { })));

        #endregion

        private static double ribbonWidth;

        public Ribbon()
        {
        }

        #region Setters/Getters Method

        /// <summary>
        /// The set bubble source.
        /// </summary>
        /// <param name="element">
        /// The element.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public static void SetBubbleSource(UIElement element, UserControl value)
        {
            element.SetValue(BubbleSourceProperty, value);
        }

        /// <summary>
        /// The get bubble source.
        /// </summary>
        /// <param name="element">
        /// The element.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static UserControl GetBubbleSource(UIElement element)
        {
            return (UserControl)element.GetValue(BubbleSourceProperty);
        }

        #endregion

        #region Override methods

        /// <summary>
        /// The on render.
        /// </summary>
        /// <param name="dc">
        /// The dc.
        /// </param>
        protected override void OnRender(DrawingContext dc)
        {      
            SolidColorBrush mySolidColorBrush = new SolidColorBrush(Color.FromRgb(222, 222, 222));
            Pen myPen = new Pen(new SolidColorBrush(Color.FromRgb(200, 22, 33)), 1);
            Rect myRect = new Rect(0, 0, double.PositiveInfinity, 120);
            dc.DrawRectangle(mySolidColorBrush, myPen, myRect);
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

        private Window GetWindow()
        {
            FrameworkElement parent = this.Parent as FrameworkElement;

            while (!(parent is Window))
            {
                parent = parent?.Parent as FrameworkElement;

                if (parent is Window w)
                {
                    return w;
                }
            }

            return null;
        }

        /// <summary>
        /// The on bubble source changed.
        /// </summary>
        /// <param name="d">
        /// The d.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void OnBubbleSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// The coerce directory path.
        /// </summary>
        /// <param name="d">
        /// The d.
        /// </param>
        /// <param name="baseValue">
        /// The base value.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        private static object CoerceBubbleSource(DependencyObject d, object baseValue)
        {
            var panel = d;
            return baseValue;
        }
    }
}
