
namespace UnityCommander.Modules.FilePanel
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    using UnityCommander.Modules.FilePanel.Behaviors;

    /// <summary>
    /// The drop target highlight adorner.
    /// </summary>
    public class DropTargetHighlightAdorner : DropTargetAdorner
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DropTargetHighlightAdorner"/> class.
        /// </summary>
        /// <param name="adornedElement">
        /// The adorned element.
        /// </param>
        public DropTargetHighlightAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DropTargetHighlightAdorner"/> class.
        /// </summary>
        /// <param name="adornedElement">
        /// The adorned element.
        /// </param>
        /// <param name="dropInfo">
        /// The drop info.
        /// </param>
        public DropTargetHighlightAdorner(UIElement adornedElement, DropInfo dropInfo)
            : base(adornedElement, dropInfo)
        {
        }

        /// <summary>
        /// The on render.
        /// </summary>
        /// <param name="drawingContext">
        /// The drawing context.
        /// </param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            var dropInfo = this.DropInfo;
            var visualTargetItem = dropInfo.VisualTargetItem;
            if (visualTargetItem != null)
            {
                var rect = Rect.Empty;

                if (visualTargetItem is ListView tvItem && VisualTreeHelper.GetChildrenCount(tvItem) > 0)
                {
                    var descendant = VisualTreeHelper.GetDescendantBounds(tvItem);
                    var translatePoint = tvItem.TranslatePoint(new Point(), this.AdornedElement);
                    var itemRect = new Rect(translatePoint, tvItem.RenderSize);
                    descendant.Union(itemRect);
                    translatePoint.Offset(1, 0);
                    rect = new Rect(translatePoint, new Size(descendant.Width - translatePoint.X - 1, tvItem.ActualHeight));
                }

                if (rect.IsEmpty)
                {
                    rect = new Rect(visualTargetItem.TranslatePoint(new Point(), this.AdornedElement), VisualTreeHelper.GetDescendantBounds(visualTargetItem).Size);
                }

                drawingContext.DrawRoundedRectangle(null, this.Pen, rect, 2, 2);
            }
        }
    }
}
