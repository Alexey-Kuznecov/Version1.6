
namespace UnityCommander.Controls.TabPanel
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    using Point = System.Windows.Point;
    using Size = System.Windows.Size;

    /// <summary>
    /// The tab control.
    /// </summary>
    [SuppressMessage("ReSharper", "StyleCop.SA1503")]
    public class TabPanel : Panel
    {
        public static readonly DependencyProperty CollectionProperty =
            DependencyProperty.Register("Collection", typeof(TabCollection), typeof(TabPanel), new PropertyMetadata(null, OnInitialElementsChangedCallback, CoerceValueCallback));

        private TabControl dragDataSource;

        public TabCollection Collection
        {
            get => (TabCollection)this.GetValue(CollectionProperty);
            set => this.SetValue(CollectionProperty, value);
        }

        protected override Size MeasureOverride(Size constraint)
        {
            Size size = new Size(double.PositiveInfinity, double.PositiveInfinity);
            foreach (UIElement child in this.Children)
            {
                child.Measure(size);
            }

            return new Size();
        }

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

        private static void OnInitialElementsChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TabPanel {Collection: { }} tabPanel)
            {
                if (tabPanel.Collection.Count > 0)
                {
                    tabPanel.Collection.CollectionChanged += tabPanel.OnCollectionChanged;

                    foreach (Control control in tabPanel.Collection)
                    {
                        tabPanel.RegisterEvent(control, tabPanel);
                        tabPanel.Children.Add(control);
                    }
                } 
            }
        }

        private void ControlOnMouseEnter(object sender, MouseEventArgs e)
        {
        }

        private void ControlOnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.dragDataSource ??= (TabControl) this.Collection.GetActive();
                System.Windows.DragDrop.DoDragDrop(new Button(), this.dragDataSource, DragDropEffects.All);
                e.Handled = true;
            }
        }

        private void ControlOnMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.dragDataSource = sender as TabControl;
        }

        private void ControlOnDragLeave(object sender, DragEventArgs e)
        {
        }

        private void ControlOnPreviewDrop(object sender, DragEventArgs e)
        {
        }

        private void ControlOnDragEnter(object sender, DragEventArgs e)
        {
            var dropDataTarget = sender as TabControl;

            if (this.dragDataSource != null)
            {
                var targetIndex = this.Children.IndexOf(dropDataTarget);
                this.Children.Remove(this.dragDataSource);
                this.Children.Insert(targetIndex, this.dragDataSource);
            }
        }

        private static object CoerceValueCallback(DependencyObject d, object value)
        {
            return value;
        }

        private void OnCollectionChanged(object sender, EventArgs e)
        {
            // Если коллекция Children больше чем коллекция пользователя, то элемент был удален и наборот.
            bool typeChanged = this.Children.Count > (sender as TabCollection).Count;

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

                    if (control.Equals(collection.Active))
                        control.IsEnabled = false;

                    this.Children.Add(control);
                }

                // Подписаться на события если был добавлен новый элемент.
                if (!typeChanged)
                    this.RegisterEvent(this.Children[this.Children.Count - 1] as TabControl, this);
                
                this.Children.Add(addControl ?? throw new InvalidOperationException());
            }
        }

        private void RegisterEvent(Control control, TabPanel tabPanel)
        {
            control.DragEnter += tabPanel.ControlOnDragEnter;
            control.DragLeave += tabPanel.ControlOnDragLeave;
            control.MouseMove += tabPanel.ControlOnPreviewMouseMove;
            control.MouseEnter += tabPanel.ControlOnMouseEnter;
            control.PreviewMouseLeftButtonDown += tabPanel.ControlOnMouseDown;
            control.PreviewDrop += tabPanel.ControlOnPreviewDrop;
        }
    }
}