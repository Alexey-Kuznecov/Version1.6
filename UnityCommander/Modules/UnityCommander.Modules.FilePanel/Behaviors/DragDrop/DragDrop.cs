
namespace UnityCommander.Modules.FilePanel.Behaviors.DragDrop
{
    using System;
    using System.Windows;
    
    /// <summary>
    /// The drag drop.
    /// </summary>
    public static partial class DragDrop
    {
        /// <summary>
        /// The scroll.
        /// </summary>
        /// <param name="dropInfo">
        /// The drop info.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void Scroll(DropInfo dropInfo, DragEventArgs e)
        {
            if (dropInfo == null || dropInfo.TargetScrollViewer == null)
            {
                return;
            }

            var scrollViewer = dropInfo.TargetScrollViewer;
            var scrollingMode = dropInfo.TargetScrollingMode;

            var position = e.GetPosition(scrollViewer);
            var scrollMargin = Math.Min(scrollViewer.FontSize * 2, scrollViewer.ActualHeight / 2);

            if (scrollingMode == ScrollingMode.Both || scrollingMode == ScrollingMode.HorizontalOnly)
            {
                if (position.X >= scrollViewer.ActualWidth - scrollMargin && scrollViewer.HorizontalOffset < scrollViewer.ExtentWidth - scrollViewer.ViewportWidth)
                {
                    scrollViewer.LineRight();
                }
                else if (position.X < scrollMargin && scrollViewer.HorizontalOffset > 0)
                {
                    scrollViewer.LineLeft();
                }
            }

            if (scrollingMode == ScrollingMode.Both || scrollingMode == ScrollingMode.VerticalOnly)
            {
                if (position.Y >= scrollViewer.ActualHeight - scrollMargin && scrollViewer.VerticalOffset < scrollViewer.ExtentHeight - scrollViewer.ViewportHeight)
                {
                    scrollViewer.LineDown();
                }
                else if (position.Y < scrollMargin && scrollViewer.VerticalOffset > 0)
                {
                    scrollViewer.LineUp();
                }
            }
        }
    }
}
