
using Components.DragDrop.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using UnityCommander.Core.DragDrop;

namespace Components.DragDrop
{
    /// <summary>
    /// The drag drop copy.
    /// </summary>
    public static partial class DragDrop
    {
        /// <summary>
        /// Gets or Sets whether the control can be used as drag source.
        /// </summary>
        public static readonly DependencyProperty IsDragSourceProperty = DependencyProperty.RegisterAttached(
            "IsDragSource", typeof(bool), typeof(DragDrop), new UIPropertyMetadata(false, IsDragSourceChanged));

        /// <summary>
        /// The effect adorner.
        /// </summary>
        private static DragAdorner effectAdorner;

        /// <summary>
        /// The _ drag adorner.
        /// </summary>
        private static DragAdorner dragAdorner;

        private static DragInfo m_DragInfo;
        private static object m_ClickSupressItem;

        /// <summary>
        /// Gets or sets the effect adorner.
        /// </summary>
        private static DragAdorner EffectAdorner
        {
            get => effectAdorner;
            set
            {
                effectAdorner?.Detatch();
                effectAdorner = value;
            }
        }

        public static object DropTargetAdorner { get; private set; }

        /// <summary>
        /// Gets or sets the drag adorner.
        /// </summary>
        private static DragAdorner DragAdorner
        {
            get => dragAdorner;
            set
            {
                dragAdorner?.Detatch();
                dragAdorner = value;
            }
        }

        /// <summary>
        /// Gets whether the control can be used as drag source.
        /// </summary>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool GetIsDragSource(UIElement target)
        {
            return (bool)target.GetValue(IsDragSourceProperty);
        }

        /// <summary>
        /// Sets whether the control can be used as drag source.
        /// </summary>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public static void SetIsDragSource(UIElement target, bool value)
        {
            target.SetValue(IsDragSourceProperty, value);
        }

        /// <summary>
        /// The is drag source changed.
        /// </summary>
        /// <param name="d">
        /// The d.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void IsDragSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var uiElement = (UIElement)d;

            if ((bool)e.NewValue)
            {
                uiElement.PreviewMouseLeftButtonDown += DragSourceOnMouseLeftButtonDown;
                uiElement.PreviewMouseLeftButtonUp += DragSourceOnMouseLeftButtonUp;
                uiElement.PreviewMouseMove += DragSourceOnMouseMove;
                uiElement.QueryContinueDrag += DragSourceOnQueryContinueDrag;
            }
            else
            {
                uiElement.PreviewMouseLeftButtonDown -= DragSourceOnMouseLeftButtonDown;
                uiElement.PreviewMouseLeftButtonUp -= DragSourceOnMouseLeftButtonUp;
                uiElement.PreviewMouseMove -= DragSourceOnMouseMove;
                uiElement.QueryContinueDrag -= DragSourceOnQueryContinueDrag;
            }
        }

        /// <summary>
        /// The drag source on mouse left button down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void DragSourceOnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DoMouseButtonDown(sender, e);
        }

        /// <summary>
        /// The drag source on mouse right button down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void DragSourceOnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            DoMouseButtonDown(sender, e);
        }

        /// <summary>
        /// The do mouse button down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1408:ConditionalExpressionsMustDeclarePrecedence", Justification = "Reviewed. Suppression is OK here.")]
        private static void DoMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            m_DragInfo = null;

            // Ignore the click if clickCount != 1 or the user has clicked on a scrollbar.
            var elementPosition = e.GetPosition((IInputElement)sender);
            if (e.ClickCount != 1
                || (sender is TabControl) && !HitTestUtilities.HitTest4Type<TabPanel>(sender, elementPosition)
                || HitTestUtilities.HitTest4Type<RangeBase>(sender, elementPosition)
                || HitTestUtilities.HitTest4Type<TextBoxBase>(sender, elementPosition)
                || HitTestUtilities.HitTest4Type<PasswordBox>(sender, elementPosition)
                || HitTestUtilities.HitTest4Type<ComboBox>(sender, elementPosition)
                || HitTestUtilities.HitTest4GridViewColumnHeader(sender, elementPosition)
                || HitTestUtilities.HitTest4DataGridTypes(sender, elementPosition)
                || HitTestUtilities.IsNotPartOfSender(sender, e))
            {
                return;
            }

            var dragInfo = new DragInfo(sender, e);

            if (dragInfo.VisualSource is ItemsControl control && control.CanSelectMultipleItems())
            {
                control.Focus();
            }

            if (dragInfo.VisualSourceItem == null)
            {
                return;
            }

            // If the sender is a list box that allows multiple selections, ensure that clicking on an 
            // already selected item does not change the selection, otherwise dragging multiple items 
            // is made impossible.
            var itemsControl = sender as ItemsControl;
            if ((Keyboard.Modifiers & ModifierKeys.Shift) == 0 && (Keyboard.Modifiers & ModifierKeys.Control) == 0 && dragInfo.VisualSourceItem != null && itemsControl != null && itemsControl.CanSelectMultipleItems())
            {
                var selectedItems = itemsControl.GetSelectedItems().OfType<object>().ToList();
                if (selectedItems.Count > 1 && selectedItems.Contains(dragInfo.SourceItem))
                {
                    m_ClickSupressItem = dragInfo.SourceItem;
                    e.Handled = true;
                }
            }

            m_DragInfo = dragInfo;
        }

        /// <summary>
        /// The drag source on mouse left button up.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void DragSourceOnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            DoMouseButtonUp(sender, e);
        }

        /// <summary>
        /// The drag source on mouse right button up.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void DragSourceOnMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            DoMouseButtonUp(sender, e);
        }

        /// <summary>
        /// The do mouse button up.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void DoMouseButtonUp(object sender, MouseButtonEventArgs e)
        {
            var elementPosition = e.GetPosition((IInputElement)sender);

            // If we prevented the control's default selection handling in DragSource_PreviewMouseLeftButtonDown
            // by setting 'e.Handled = true' and a drag was not initiated, manually set the selection here.
            if (sender is ItemsControl itemsControl)
            {
                if ((Keyboard.Modifiers & ModifierKeys.Control) != 0)
                {
                    // itemsControl.SetItemSelected(dragInfo.SourceItem, false);
                }
                else if ((Keyboard.Modifiers & ModifierKeys.Shift) == 0)
                {
                    // itemsControl.SetSelectedItem(dragInfo.SourceItem);
                }
            }
        }

        /// <summary>
        /// The drag source on mouse move.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void DragSourceOnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (sender is ItemsControl control)
                {
                    // Package the data.
                    DataObject data = new DataObject();
                    data.SetData(data: control);
                    //data.SetData("Object", new DragDropHandler());

                    // Inititate the drag-and-drop operation.
                    System.Windows.DragDrop.DoDragDrop(control, data: data, allowedEffects: DragDropEffects.Copy | DragDropEffects.Move);
                }
            }
        }

        /// <summary>
        /// The drag source on query continue drag.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void DragSourceOnQueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            if (e.Action == DragAction.Cancel || e.EscapePressed)
            {
                DragAdorner = null;
                EffectAdorner = null;
                DropTargetAdorner = null;
                Mouse.OverrideCursor = null;
            }
        }
    }
}