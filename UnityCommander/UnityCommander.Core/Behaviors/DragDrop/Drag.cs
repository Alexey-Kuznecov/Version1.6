//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Windows;
//using System.Windows.Input;

//namespace UnityCommander.Core.DragDrop
//{
//    using System.Diagnostics.CodeAnalysis;
//    using System.Linq;
//    using System.Windows;
//    using System.Windows.Controls;
//    using System.Windows.Controls.Primitives;
//    using System.Windows.Documents;
//    using System.Windows.Input;
//    using System.Windows.Media;
//    using Utilities;

//    /// <summary>
//    /// The drag drop copy.
//    /// </summary>
//    public static partial class DragDrop
//    {
//        private static bool m_DragInProgress;
//        private static Point _adornerPos;
//        private static Size _adornerSize;

//        private static object m_ClickSupressItem;

//        private static DragInfo m_DragInfo;

//        /// <summary>
//        /// The effect adorner.
//        /// </summary>
//        private static DragAdorner effectAdorner;

//        /// <summary>
//        /// The _ drag adorner.
//        /// </summary>
//        private static DragAdorner dragAdorner;

//        /// <summary>
//        /// Gets or sets the effect adorner.
//        /// </summary>
//        private static DragAdorner EffectAdorner
//        {
//            get => effectAdorner;
//            set
//            {
//                effectAdorner?.Detatch();
//                effectAdorner = value;
//            }
//        }

//        public static DropTargetAdorner DropTargetAdorner { get; private set; }

//        /// <summary>
//        /// Gets or sets the drag adorner.
//        /// </summary>
//        private static DragAdorner DragAdorner
//        {
//            get => dragAdorner;
//            set
//            {
//                dragAdorner?.Detatch();
//                dragAdorner = value;
//            }
//        }

//        /// <summary>
//        /// The drag source on mouse left button down.
//        /// </summary>
//        /// <param name="sender">
//        /// The sender.
//        /// </param>
//        /// <param name="e">
//        /// The e.
//        /// </param>
//        private static void DragSourceOnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
//        {
//            DoMouseButtonDown(sender, e);
//        }

//        /// <summary>
//        /// The drag source on mouse right button down.
//        /// </summary>
//        /// <param name="sender">
//        /// The sender.
//        /// </param>
//        /// <param name="e">
//        /// The e.
//        /// </param>
//        private static void DragSourceOnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
//        {
//            DoMouseButtonDown(sender, e);
//        }

//        /// <summary>
//        /// The do mouse button down.
//        /// </summary>
//        /// <param name="sender">
//        /// The sender.
//        /// </param>
//        /// <param name="e">
//        /// The e.
//        /// </param>
//        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1408:ConditionalExpressionsMustDeclarePrecedence", Justification = "Reviewed. Suppression is OK here.")]
//        private static void DoMouseButtonDown(object sender, MouseButtonEventArgs e)
//        {
//            m_DragInfo = null;

//            // Ignore the click if clickCount != 1 or the user has clicked on a scrollbar.
//            var elementPosition = e.GetPosition((IInputElement)sender);
//            if (e.ClickCount != 1
//                || (sender as UIElement).IsDragSourceIgnored()
//                || (e.Source as UIElement).IsDragSourceIgnored()
//                || (e.OriginalSource as UIElement).IsDragSourceIgnored()
//                || (sender is TabControl) && !HitTestUtilities.HitTest4Type<TabPanel>(sender, elementPosition)
//                || HitTestUtilities.HitTest4Type<RangeBase>(sender, elementPosition)
//                || HitTestUtilities.HitTest4Type<TextBoxBase>(sender, elementPosition)
//                || HitTestUtilities.HitTest4Type<PasswordBox>(sender, elementPosition)
//                || HitTestUtilities.HitTest4Type<ComboBox>(sender, elementPosition)
//                || HitTestUtilities.HitTest4GridViewColumnHeader(sender, elementPosition)
//                || HitTestUtilities.HitTest4DataGridTypes(sender, elementPosition)
//                || HitTestUtilities.IsNotPartOfSender(sender, e))
//            {
//                return;
//            }

//            var dragInfo = new DragInfo(sender, e);

//            if (dragInfo.VisualSource is ItemsControl control && control.CanSelectMultipleItems())
//            {
//                control.Focus();
//            }

//            if (dragInfo.VisualSourceItem == null)
//            {
//                return;
//            }

//            var dragHandler = TryGetDragHandler(dragInfo, sender as UIElement);
//            if (!dragHandler.CanStartDrag(dragInfo))
//            {
//                return;
//            }

//            // If the sender is a list box that allows multiple selections, ensure that clicking on an 
//            // already selected item does not change the selection, otherwise dragging multiple items 
//            // is made impossible.
//            var itemsControl = sender as ItemsControl;
//            if ((Keyboard.Modifiers & ModifierKeys.Shift) == 0 && (Keyboard.Modifiers & ModifierKeys.Control) == 0 && dragInfo.VisualSourceItem != null && itemsControl != null && itemsControl.CanSelectMultipleItems())
//            {
//                var selectedItems = itemsControl.GetSelectedItems().OfType<object>().ToList();
//                if (selectedItems.Count > 1 && selectedItems.Contains(dragInfo.SourceItem))
//                {
//                    m_ClickSupressItem = dragInfo.SourceItem;
//                    e.Handled = true;
//                }
//            }

//            m_DragInfo = dragInfo;
//        }

//        /// <summary>
//        /// The drag source on mouse left button up.
//        /// </summary>
//        /// <param name="sender">
//        /// The sender.
//        /// </param>
//        /// <param name="e">
//        /// The e.
//        /// </param>
//        private static void DragSourceOnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
//        {
//            DoMouseButtonUp(sender, e);
//        }

//        /// <summary>
//        /// The drag source on mouse right button up.
//        /// </summary>
//        /// <param name="sender">
//        /// The sender.
//        /// </param>
//        /// <param name="e">
//        /// The e.
//        /// </param>
//        private static void DragSourceOnMouseRightButtonUp(object sender, MouseButtonEventArgs e)
//        {
//            DoMouseButtonUp(sender, e);
//        }

//        /// <summary>
//        /// The do mouse button up.
//        /// </summary>
//        /// <param name="sender">
//        /// The sender.
//        /// </param>
//        /// <param name="e">
//        /// The e.
//        /// </param>
//        private static void DoMouseButtonUp(object sender, MouseButtonEventArgs e)
//        {
//            var elementPosition = e.GetPosition((IInputElement)sender);
//            if ((sender is TabControl) && !HitTestUtilities.HitTest4Type<TabPanel>(sender, elementPosition))
//            {
//                m_DragInfo = null;
//                m_ClickSupressItem = null;
//                return;
//            }

//            var dragInfo = m_DragInfo;

//            // If we prevented the control's default selection handling in DragSource_PreviewMouseLeftButtonDown
//            // by setting 'e.Handled = true' and a drag was not initiated, manually set the selection here.
//            var itemsControl = sender as ItemsControl;
//            if (itemsControl != null && dragInfo != null && m_ClickSupressItem != null && m_ClickSupressItem == dragInfo.SourceItem)
//            {
//                if ((Keyboard.Modifiers & ModifierKeys.Control) != 0)
//                {
//                    itemsControl.SetItemSelected(dragInfo.SourceItem, false);
//                }
//                else if ((Keyboard.Modifiers & ModifierKeys.Shift) == 0)
//                {
//                    itemsControl.SetSelectedItem(dragInfo.SourceItem);
//                }
//            }

//            m_DragInfo = null;
//            m_ClickSupressItem = null;
//        }

//        /// <summary>
//        /// The drag source on mouse move.
//        /// </summary>
//        /// <param name="sender">
//        /// The sender.
//        /// </param>
//        /// <param name="e">
//        /// The e.
//        /// </param>
//        private static void DragSourceOnMouseMove(object sender, MouseEventArgs e)
//        {
//            var dragInfo = m_DragInfo;
//            if (dragInfo != null && !m_DragInProgress)
//            {
//                // the start from the source
//                var dragStart = dragInfo.DragStartPosition;

//                // do nothing if mouse left/right button is released or the pointer is captured
//                if (dragInfo.MouseButton == MouseButton.Left && e.LeftButton == MouseButtonState.Released)
//                {
//                    m_DragInfo = null;
//                    return;
//                }
//                if (DragDrop.GetCanDragWithMouseRightButton(dragInfo.VisualSource) && dragInfo.MouseButton == MouseButton.Right && e.RightButton == MouseButtonState.Released)
//                {
//                    m_DragInfo = null;
//                    return;
//                }

//                // current mouse position
//                var position = e.GetPosition((IInputElement)sender);

//                // prevent selection changing while drag operation
//                dragInfo.VisualSource?.ReleaseMouseCapture();

//                // only if the sender is the source control and the mouse point differs from an offset
//                if (dragInfo.VisualSource == sender
//                    && (Math.Abs(position.X - dragStart.X) > DragDrop.GetMinimumHorizontalDragDistance(dragInfo.VisualSource) ||
//                        Math.Abs(position.Y - dragStart.Y) > DragDrop.GetMinimumVerticalDragDistance(dragInfo.VisualSource)))
//                {
//                    dragInfo.RefreshSelectedItems(sender, e);

//                    var dragHandler = TryGetDragHandler(dragInfo, sender as UIElement);
//                    if (dragHandler.CanStartDrag(dragInfo))
//                    {
//                        dragHandler.StartDrag(dragInfo);

//                        if (dragInfo.Effects != DragDropEffects.None)
//                        {
//                            var dataObject = dragInfo.DataObject;

//                            if (dataObject == null)
//                            {
//                                if (dragInfo.Data == null)
//                                {
//                                    // it's bad if the Data is null, cause the DataObject constructor will raise an ArgumentNullException
//                                    m_DragInfo = null; // maybe not necessary or should not set here to null
//                                    return;
//                                }
//                                dataObject = new DataObject(dragInfo.DataFormat.Name, dragInfo.Data);
//                            }

//                            try
//                            {
//                                m_DragInProgress = true;
//                                var dragDropHandler = dragInfo.DragDropHandler ?? System.Windows.DragDrop.DoDragDrop;
//                                var dragDropEffects = dragDropHandler(dragInfo.VisualSource, dataObject, dragInfo.Effects);
//                                if (dragDropEffects == DragDropEffects.None)
//                                {
//                                    dragHandler.DragCancelled();
//                                }
//                                dragHandler.DragDropOperationFinished(dragDropEffects, dragInfo);
//                            }
//                            catch (Exception ex)
//                            {
//                                if (!dragHandler.TryCatchOccurredException(ex))
//                                {
//                                    throw;
//                                }
//                            }
//                            finally
//                            {
//                                m_DragInProgress = false;
//                                m_DragInfo = null;
//                            }
//                        }
//                    }
//                }
//            }
//        }

//        /// <summary>
//        /// The drag source on query continue drag.
//        /// </summary>
//        /// <param name="sender">
//        /// The sender.
//        /// </param>
//        /// <param name="e">
//        /// The e.
//        /// </param>
//        private static void DragSourceOnQueryContinueDrag(object sender, QueryContinueDragEventArgs e)
//        {
//            if (e.Action == DragAction.Cancel || e.EscapePressed)
//            {
//                DragAdorner = null;
//                EffectAdorner = null;
//                DropTargetAdorner = null;
//                Mouse.OverrideCursor = null;
//            }
//        }

//        /// <summary>
//        /// Gets the drag handler from the drag info or from the sender, if the drag info is null
//        /// </summary>
//        /// <param name="dragInfo">the drag info object</param>
//        /// <param name="sender">the sender from an event, e.g. mouse down, mouse move</param>
//        /// <returns></returns>
//        private static IDragSource TryGetDragHandler(DragInfo dragInfo, UIElement sender)
//        {
//            IDragSource dragHandler = null;
//            if (dragInfo != null && dragInfo.VisualSource != null)
//            {
//                dragHandler = GetDragHandler(dragInfo.VisualSource);
//            }
//            if (dragHandler == null && sender != null)
//            {
//                dragHandler = GetDragHandler(sender);
//            }
//            return dragHandler ?? DefaultDragHandler;
//        }
//    }
//}
