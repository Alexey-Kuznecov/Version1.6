using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Components.Tab
{
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

        private TabControl dragDataSource;

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
            //dragDataSource = sender as TabControl;
        }

        private void ControlOnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                dragDataSource ??= (TabControl) this.Collection.GetActive();
                System.Windows.DragDrop.DoDragDrop(new Button(), dragDataSource, DragDropEffects.All);
                e.Handled = true;
            }
        }

        private void ControlOnMouseDown(object sender, MouseButtonEventArgs e)
        {
            dragDataSource = sender as TabControl;
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

            if (dragDataSource != null)
            {
                var targetIndex = this.Children.IndexOf(dropDataTarget);
                this.Children.Remove(dragDataSource);
                this.Children.Insert(targetIndex, dragDataSource);
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
        private void OnCollectionChanged(object sender, EventArgs e)
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

                    if (control.Equals(collection.Active))
                    {
                        control.IsEnabled = false;
                    }

                    this.Children.Add(control);
                }

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