
namespace UnityCommander.Controls.TabPanel.DragDrop
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;

    public static partial class DragDrop
    {
        private static RepeatButton dragDataSource;

        private static void DoMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            dragDataSource = sender as RepeatButton;
        }

        private static void DoMouseButtonUp(object sender, MouseButtonEventArgs e)
        {
        }

        private static void DragSourceOnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                System.Windows.DragDrop.DoDragDrop(new Button(), dragDataSource, DragDropEffects.All);
                e.Handled = true;
            }
        }

        private static void Drop(object sender, DragEventArgs e)
        {
            //System.Console.WriteLine("Dropped: " + e.Data.GetData(typeof(Int32)).ToString());
        }

        private static void DragSourceOnQueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
        }

        private static void DropTargetOnDragLeave(object sender, DragEventArgs e)
        {
            var data = e.Data.GetData(typeof(RepeatButton));
        }

        private static void DropTargetOnDragOver(object sender, DragEventArgs e)
        {
            var data = e.Data.GetData(typeof(RepeatButton));
        }

        private static void DropTargetOnDrop(object sender, DragEventArgs e)
        {
            var source = (RepeatButton)e.Data.GetData(typeof(RepeatButton));

            var parent = source?.Parent;

            var target = sender;
        }

        private static void DropTargetOnGiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
        }

        private static void DragSourceOnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DoMouseButtonDown(sender, e);
        }

        private static void DragSourceOnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            DoMouseButtonDown(sender, e);
        }

        private static void DragSourceOnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            DoMouseButtonUp(sender, e);
        }

        private static void DragSourceOnMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            DoMouseButtonUp(sender, e);
        }

        private static void DropTargetOnDragEnter(object sender, DragEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private static void DropTargetOnPreviewDrop(object sender, DragEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private static void DropTargetOnPreviewDragOver(object sender, DragEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private static void DropTargetOnPreviewDragEnter(object sender, DragEventArgs e)
        {
            //throw new NotImplementedException();
        }
    }
}
