
namespace UnityCommander.Controls.Ribbon
{
    using System;
    using System.Collections.ObjectModel;
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

        /// <summary>
        /// The tab controls.
        /// </summary>
        private UIElementCollection tabControls;

        /// <summary>
        /// The tab controls.
        /// </summary>
        private UIElementCollection ribbonContainer;

        /// <summary>
        /// The window width.
        /// </summary>
        private Size windowSize;

        /// <summary>
        /// Gets or sets the set command.
        /// </summary>
        public ICommand SetCommand
        {
            get => (ICommand)this.GetValue(SetCommandProperty);
            set => this.SetValue(SetCommandProperty, value);
        }

        /// <summary>
        /// Gets or sets the tab command.
        /// </summary>
        public ICommand TabCommand => new RelayCommand(obj =>
        {
            foreach (var tab in this.tabControls)
            {
                if (!(tab is Button child)) continue;
                if (obj is Button bt)
                {
                    child.IsEnabled = child.GetHashCode() != bt.GetHashCode();

                    if (!child.IsEnabled)
                    {
                        SetCommand.Execute(bt);
                    }
                }
            }
        });

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
            FrameworkElement parent = this.Parent as FrameworkElement;

            while (!(parent is Window))
            {
                parent = parent?.Parent as FrameworkElement;

                if (parent is Window w)
                {
                    this.windowSize = new Size(w.Width, w.Height);
                    SolidColorBrush mySolidColorBrush = new SolidColorBrush(Color.FromRgb(222, 222, 222));
                    Pen myPen = new Pen(new SolidColorBrush(Color.FromRgb(200, 22, 33)), 1);
                    Rect myRect = new Rect(0, 0, double.PositiveInfinity, 120);
                    dc.DrawRectangle(mySolidColorBrush, myPen, myRect);
                }
            }
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

            foreach (UIElement child in this.InternalChildren)
            {
                child.Arrange(new Rect(new Point(margin, 0), child.DesiredSize));
                margin += child.DesiredSize.Width;
                this.tabControls = ((RibbonTaber)((Grid)child).Children[0])?.Children;
                this.ribbonContainer = ((RibbonContainer)((Grid)child).Children[1])?.Children;

                if (this.tabControls == null) continue;

                foreach (var tab in this.tabControls)
                {
                    if (tab is Button bt)
                    {
                        bt.Command = this.TabCommand;
                        bt.CommandParameter = bt;
                    }
                }
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
            foreach (UIElement child in this.InternalChildren)
            {
                child.Measure(size);
            }

            return new Size();
        }

        #endregion

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
            if (d is Ribbon ribbon)
            {
                if (ribbon.ribbonContainer == null) return;
                ribbon.ribbonContainer.Clear();
                ribbon.ribbonContainer.Add(
                    new ContentControl
                        {
                            Content = GetBubbleSource(ribbon),
                            Width = ribbon.windowSize.Width,
                            Height = 100
                        });
            }
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
