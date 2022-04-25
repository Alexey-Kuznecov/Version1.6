using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace UnityCommander.Controls.Taber.DragDrop
{
    public static partial class DragDrop
    {
        #region IsDragSourceProperty

        /// <summary>
        /// Gets or Sets whether the control can be used as drag source.
        /// </summary>
        public static readonly DependencyProperty IsDragSourceProperty
            = DependencyProperty.RegisterAttached("IsDragSource",
                                                  typeof(bool),
                                                  typeof(DragDrop),
                                                  new UIPropertyMetadata(false, IsDragSourceChanged));

        /// <summary>
        /// Gets whether the control can be used as drag source.
        /// </summary>
        public static bool GetIsDragSource(UIElement target)
        {
            return (bool)target.GetValue(IsDragSourceProperty);
        }

        /// <summary>
        /// Sets whether the control can be used as drag source.
        /// </summary>
        public static void SetIsDragSource(UIElement target, bool value)
        {
            target.SetValue(IsDragSourceProperty, value);
        }

        private static void IsDragSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var uiElement = (UIElement)d;

            if ((bool)e.NewValue)
            {
                uiElement.DragEnter += DropTargetOnDragEnter;
                uiElement.DragLeave += DropTargetOnDragLeave;
                uiElement.DragOver += DropTargetOnDragOver;
                uiElement.Drop += DropTargetOnDrop;
                uiElement.GiveFeedback += DropTargetOnGiveFeedback;
                uiElement.PreviewMouseDown += DoMouseButtonDown;
                uiElement.PreviewMouseUp += DoMouseButtonUp;
                uiElement.PreviewMouseLeftButtonDown += DragSourceOnMouseLeftButtonDown;
                uiElement.PreviewMouseLeftButtonUp += DragSourceOnMouseLeftButtonUp;
                uiElement.PreviewMouseMove += DragSourceOnMouseMove;
                uiElement.QueryContinueDrag += DragSourceOnQueryContinueDrag;
                uiElement.PreviewDragEnter += DropTargetOnPreviewDragEnter;
                uiElement.PreviewDragLeave += DropTargetOnDragLeave;
                uiElement.PreviewDragOver += DropTargetOnPreviewDragOver;
                uiElement.PreviewDrop += DropTargetOnPreviewDrop;
                uiElement.PreviewGiveFeedback += DropTargetOnGiveFeedback;
            }
            else
            {
                uiElement.DragEnter -= DropTargetOnDragEnter;
                uiElement.DragLeave -= DropTargetOnDragLeave;
                uiElement.DragOver -= DropTargetOnDragOver;
                uiElement.Drop -= DropTargetOnDrop;
                uiElement.GiveFeedback -= DropTargetOnGiveFeedback;
                uiElement.PreviewMouseDown -= DoMouseButtonDown;
                uiElement.PreviewMouseUp -= DoMouseButtonUp;
                uiElement.PreviewMouseLeftButtonDown -= DragSourceOnMouseLeftButtonDown;
                uiElement.PreviewMouseLeftButtonUp -= DragSourceOnMouseLeftButtonUp;
                uiElement.PreviewMouseMove -= DragSourceOnMouseMove;
                uiElement.QueryContinueDrag -= DragSourceOnQueryContinueDrag;
                uiElement.PreviewDragEnter -= DropTargetOnPreviewDragEnter;
                uiElement.PreviewDragLeave -= DropTargetOnDragLeave;
                uiElement.PreviewDragOver -= DropTargetOnPreviewDragOver;
                uiElement.PreviewDrop -= DropTargetOnPreviewDrop;
                uiElement.PreviewGiveFeedback -= DropTargetOnGiveFeedback;
            }
        }

        #endregion

        #region IsDropTargetProperty

        /// <summary>
        /// Gets or Sets whether the control can be used as drop target.
        /// </summary>
        public static readonly DependencyProperty IsDropTargetProperty
            = DependencyProperty.RegisterAttached("IsDropTarget",
                                                  typeof(bool),
                                                  typeof(DragDrop),
                                                  new UIPropertyMetadata(false, IsDropTargetChanged));

        /// <summary>
        /// Gets whether the control can be used as drop target.
        /// </summary>
        public static bool GetIsDropTarget(UIElement target)
        {
            return (bool)target.GetValue(IsDropTargetProperty);
        }

        /// <summary>
        /// Sets whether the control can be used as drop target.
        /// </summary>
        public static void SetIsDropTarget(UIElement target, bool value)
        {
            target.SetValue(IsDropTargetProperty, value);
        }

        private static void IsDropTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var uiElement = (UIElement)d;

            if ((bool)e.NewValue)
            {
                uiElement.AllowDrop = true;

                RegisterDragDropEvents(uiElement);
            }
            else
            {
                uiElement.AllowDrop = false;

                UnregisterDragDropEvents(uiElement);

                Mouse.OverrideCursor = null;
            }
        }

        #endregion

        private static void RegisterDragDropEvents(UIElement uiElement)
        {
            // use normal events for ItemsControls
            uiElement.DragEnter += DropTargetOnDragEnter;
            uiElement.DragLeave += DropTargetOnDragLeave;
            uiElement.DragOver += DropTargetOnDragOver;
            uiElement.Drop += DropTargetOnDrop;
            uiElement.GiveFeedback += DropTargetOnGiveFeedback;

            // issue #85: try using preview events for all other elements than ItemsControls
            uiElement.PreviewDragEnter += DropTargetOnPreviewDragEnter;
            uiElement.PreviewDragLeave += DropTargetOnDragLeave;
            uiElement.PreviewDragOver += DropTargetOnPreviewDragOver;
            uiElement.PreviewDrop += DropTargetOnPreviewDrop;
            uiElement.PreviewGiveFeedback += DropTargetOnGiveFeedback;
        }

        private static void UnregisterDragDropEvents(UIElement uiElement)
        {
            // use normal events for ItemsControls
            uiElement.DragEnter -= DropTargetOnDragEnter;
            uiElement.DragLeave -= DropTargetOnDragLeave;
            uiElement.DragOver -= DropTargetOnDragOver;
            uiElement.Drop -= DropTargetOnDrop;
            uiElement.GiveFeedback -= DropTargetOnGiveFeedback;

            // issue #85: try using preview events for all other elements than ItemsControls
            uiElement.PreviewDragEnter -= DropTargetOnPreviewDragEnter;
            uiElement.PreviewDragLeave -= DropTargetOnDragLeave;
            uiElement.PreviewDragOver -= DropTargetOnPreviewDragOver;
            uiElement.PreviewDrop -= DropTargetOnPreviewDrop;
            uiElement.PreviewGiveFeedback -= DropTargetOnGiveFeedback;
        }
    }
}
