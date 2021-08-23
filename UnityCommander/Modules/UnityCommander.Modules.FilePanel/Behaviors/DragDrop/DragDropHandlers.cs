
namespace UnityCommander.Modules.FilePanel.Behaviors.DragDrop
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    /// <summary>
    /// The drag drop.
    /// </summary>
    public class DragDropHandler
    {
        /// <summary>
        /// Gets or sets the mouse move handler.
        /// </summary>
        public static MouseEventHandler MouseMoveHandler { get; set; } = OnMouseMove;

        /// <summary>
        /// Gets or sets the drop handler.
        /// </summary>
        public static DragEventHandler DropHandler { get; set; } = OnDrop;

        /// <summary>
        /// Gets or sets the drag over handler.
        /// </summary>
        public static DragEventHandler DragOverHandler { get; set; } = OnDragOver;

        /// <summary>
        /// Gets or sets the drag leave handler.
        /// </summary>
        public static DragEventHandler DragLeaveHandler { get; set; } = OnDragLeave;

        /// <summary>
        /// Gets or sets the drag enter handler.
        /// </summary>
        public static DragEventHandler DragEnterHandler { get; set; } = OnDragEnter;

        /// <summary>
        /// Gets or sets the give feedback handler.
        /// </summary>
        public static GiveFeedbackEventHandler GiveFeedbackHandler { get; set; } = OnGiveFeedback;


        /// <summary>
        /// The on mouse move.
        /// </summary>
        /// <param name="sender">The sender. </param>
        /// <param name="e"> The e. </param>
        public static void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (sender is ContentControl control)
                {
                    // Package the data.
                    DataObject data = new DataObject();
                    data.SetData(data: control);
                    data.SetData("Object", new DragDropHandler());

                    // Inititate the drag-and-drop operation.
                    System.Windows.DragDrop.DoDragDrop(control, data: data, allowedEffects: DragDropEffects.Copy | DragDropEffects.Move);
                }
            }
        }

        /// <summary>
        /// The on drop.
        /// </summary>
        /// <param name="sender">The sender. </param>
        /// <param name="e"> The e. </param>
        public static void OnDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetData(typeof(ContentControl)) is ContentControl)
            {
                if (sender is TextBlock textBlock)
                {
                    textBlock.Style = null;
                }

                // IconCollectionBase.DragDrop.Invoke(sender, e);
                
                // Set Effects to notify the drag source what effect
                // the drag-and-drop operation had.
                // (Copy if CTRL is pressed; otherwise, move.)
                if (e.KeyStates.HasFlag(DragDropKeyStates.ControlKey))
                {
                    e.Effects = DragDropEffects.Copy;
                }
                else
                {
                    e.Effects = DragDropEffects.Move;
                }
            }

            e.Handled = true;
        }

        /// <summary>
        /// The on drag over.
        /// </summary>
        /// <param name="sender">The sender. </param>
        /// <param name="e"> The e. </param>
        public static void OnDragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.None;

            if (sender is TextBlock textBlock)
            {
                textBlock.Style = (Style)Application.Current.TryFindResource("CollectionMenuDragDropStyle");
            }

            // Set Effects to notify the drag source what effect
            // the drag-and-drop operation will have. These values are 
            // used by the drag source's GiveFeedback event handler.
            // (Copy if CTRL is pressed; otherwise, move.)
            if (e.KeyStates.HasFlag(DragDropKeyStates.ControlKey))
            {
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.Move;
            }

            e.Handled = true;
        }

        /// <summary>
        /// The on drag enter.
        /// </summary>
        /// <param name="sender">The sender. </param>
        /// <param name="e"> The e. </param>
        public static void OnDragEnter(object sender, DragEventArgs e)
        {
            var button = e.Data.GetData(typeof(Button));

            if (button != null)
            {
                var dds = sender as TextBox;
                var collection = dds?.Text;
            }
        }

        /// <summary>
        /// The on give feedback.
        /// </summary>
        /// <param name="sender">The sender. </param>
        /// <param name="e"> The e. </param>
        public static void OnGiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            // These Effects values are set in the drop target's
            // DragOver event handler.
            if (e.Effects.HasFlag(DragDropEffects.Copy))
            {
                Mouse.SetCursor(Cursors.Cross);
            }
            else if (e.Effects.HasFlag(DragDropEffects.Move))
            {
                Mouse.SetCursor(Cursors.UpArrow);
            }
            else
            {
                Mouse.SetCursor(Cursors.No);
            }

            e.Handled = true;
        }

        /// <summary>
        /// The on drag leave.
        /// </summary>
        /// <param name="sender">The sender. </param>
        /// <param name="e"> The e. </param>
        public static void OnDragLeave(object sender, DragEventArgs e)
        {
            if (sender is TextBlock textBlock)
            {
                textBlock.Style = null;
            }
        }
    }
}
