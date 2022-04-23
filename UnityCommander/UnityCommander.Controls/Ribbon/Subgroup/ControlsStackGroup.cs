
namespace UnityCommander.Controls.Ribbon.Subgroup
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    /// <summary>
    /// The controls stack group.
    /// </summary>
    public class ControlsStackGroup : Panel
    {
        #region Fields

        /// <summary>
        /// The dummy drag source.
        /// </summary>
        private readonly UIElement dummyDragSource = new ();

        /// <summary>
        /// The is down.
        /// </summary>
        private bool isDown;

        /// <summary>
        /// The is dragging.
        /// </summary>
        private bool isDragging;

        /// <summary>
        /// The start point.
        /// </summary>
        private Point startPoint;

        /// <summary>
        /// The real drag source.
        /// </summary>
        private UIElement realDragSource;

        /// <summary>
        /// The margin.
        /// </summary>
        private double margin;

        /// <summary>
        /// The container group width.
        /// </summary>
        private Size containerGroupWidth;

        /// <summary>
        /// The container group width.
        /// </summary>
        private int nextColumn = 5;

        /// <summary>
        /// The container group width.
        /// </summary>
        private int itemNumber;

        /// <summary>
        /// The container group width.
        /// </summary>
        private double verticalAl;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ControlsStackGroup"/> class.
        /// </summary>
        public ControlsStackGroup()
        {
            this.Margin = new Thickness(0, 5, 0, 0);
            this.AllowDrop = true;
            this.Drop += ItemPanel_OnPreviewDrop;
            this.DragEnter += this.ItemPanel_OnPreviewDragEnter;
        }

        #region Override Members

        /// <summary>
        /// The on apply template.
        /// </summary>
        public override void OnApplyTemplate()
        {
            this.Margin = new Thickness(10, 5, 0, 0);
            base.OnApplyTemplate();
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
            this.verticalAl = 0f;

            foreach (UIElement child in this.Children)
            {
                child.Arrange(new Rect(new Point(this.margin, this.verticalAl), child.DesiredSize));

                if (this.itemNumber >= this.nextColumn)
                {
                    this.verticalAl = 0f;
                    this.margin += child.DesiredSize.Width + 5f;
                }

                this.verticalAl += child.DesiredSize.Height;
                this.itemNumber++;
            }

            return arrangeBounds;
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
            double width = 0;
            double height = 0;

            Size size = new Size(double.PositiveInfinity, double.PositiveInfinity);
            foreach (UIElement child in this.Children)
            {
                child.Measure(size);
                height += child.DesiredSize.Height;
                width = child.DesiredSize.Width;
            }

            this.containerGroupWidth = new Size(width, height);
            return this.containerGroupWidth;
        }

        #endregion

        #region Drag and Drop

        /// <summary>
        /// The on preview mouse left button down.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (Equals(e.Source, this))
            {
            }
            else
            {
                this.isDown = true;
                this.startPoint = e.GetPosition(this);
            }
        }

        /// <summary>
        /// The on preview mouse left button up.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            this.isDown = false;
            this.isDragging = false;
            this.realDragSource?.ReleaseMouseCapture();
        }

        /// <summary>
        /// The on preview mouse move.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            if (this.isDown)
            {
                if (this.isDragging
                    || !(Math.Abs(e.GetPosition(this).X - this.startPoint.X)
                         > SystemParameters.MinimumHorizontalDragDistance)
                    && !(Math.Abs(e.GetPosition(this).Y - this.startPoint.Y)
                         > SystemParameters.MinimumVerticalDragDistance))
                {
                    return;
                }

                this.isDragging = true;
                this.realDragSource = e.Source as UIElement;
                this.realDragSource?.CaptureMouse();
                DragDrop.DoDragDrop(this.dummyDragSource, new DataObject("UIElement", e.Source, true), DragDropEffects.Copy);
            }
        }

        /// <summary>
        /// The Item Panel on preview drag enter.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void ItemPanel_OnPreviewDragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("UIElement"))
            {
                e.Effects = DragDropEffects.Move;
            }
        }

        /// <summary>
        /// The Item Panel on preview drop.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void ItemPanel_OnPreviewDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("UIElement"))
            {
                var dataTarget = e.Source as FrameworkElement;
                int dropTargetIndex = -1, i = 0;
                var dataSource = default(FrameworkElement); // (FrameworkElement)e.Data.GetFormats(true).Select(d => e.Data.GetData(d));
                var sourcePanel = default(Panel);

                foreach (string format in e.Data.GetFormats(true))
                {
                    dataSource = (FrameworkElement)e.Data.GetData(format);
                    sourcePanel = dataSource?.Parent as Panel;
                }

                // Get element index
                foreach (UIElement element in this.Children)
                {
                    if (element.Equals(dataTarget))
                    {
                        dropTargetIndex = i;
                        break;
                    }
                    i++;
                }

                if (dropTargetIndex != -1)
                {
                    if (sourcePanel != null && sourcePanel.Equals(this))
                    {
                        this.Children.Remove(this.realDragSource);
                        this.Children.Insert(dropTargetIndex, this.realDragSource);
                    }
                    else
                    {
                        sourcePanel?.Children.Remove(dataSource);
                        this.Children.Insert(dropTargetIndex, dataSource ?? throw new InvalidOperationException());
                    }
                }

                this.isDown = false;
                this.isDragging = false;
                this.realDragSource?.ReleaseMouseCapture();
            }
        }

        #endregion
    }
}
