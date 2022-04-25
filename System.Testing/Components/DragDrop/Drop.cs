
using System;
using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using UnityCommander.Core.DragDrop;
using UnityCommander.Core.DragDrop.Utilities;

namespace Components.DragDrop
{
    /// <summary>
    /// The drag.
    /// </summary>
    public static partial class DragDrop
    {
        /// <summary>
        /// Gets or Sets whether the control can be used as drop target.
        /// </summary>
        public static readonly DependencyProperty IsDropTargetProperty = DependencyProperty.RegisterAttached(
            "IsDropTarget", typeof(bool), typeof(DragDrop), new UIPropertyMetadata(false, IsDropTargetChanged));

        /// <summary>
        /// Gets or sets the events which are subscribed for the drag and drop events
        /// </summary>
        public static readonly DependencyProperty DropEventTypeProperty =
            DependencyProperty.RegisterAttached("DropEventType", typeof(EventType), typeof(DragDrop), new PropertyMetadata(EventType.Auto, DropEventTypeChanged));

        /// <summary>
        /// Gets whether the control can be used as drop target.
        /// </summary>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool GetIsDropTarget(UIElement target)
        {
            return (bool)target.GetValue(IsDropTargetProperty);
        }

        /// <summary>
        /// Sets whether the control can be used as drop target.
        /// </summary>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public static void SetIsDropTarget(UIElement target, bool value)
        {
            target.SetValue(IsDropTargetProperty, value);
        }

        /// <summary>
        /// Gets which type of events are subscribed for the drag and drop events.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <returns>
        /// The <see cref="EventType"/>.
        /// </returns>
        public static EventType GetDropEventType(DependencyObject obj)
        {
            return (EventType)obj.GetValue(DropEventTypeProperty);
        }

        /// <summary>
        /// Sets which type of events are subscribed for the drag and drop events.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public static void SetDropEventType(DependencyObject obj, EventType value)
        {
            obj.SetValue(DropEventTypeProperty, value);
        }

        #region Register and Unregister 

        /// <summary>
        /// The unregister drag drop events.
        /// </summary>
        /// <param name="element">
        /// The element.
        /// </param>
        /// <param name="eventType">
        /// The event type.
        /// </param>
        private static void UnregisterDragDropEvents(UIElement element, EventType eventType)
        {
            switch (eventType)
            {
                case EventType.Auto:
                    if (element is ItemsControl)
                    {
                        // use normal events for ItemsControls
                        element.DragEnter -= DropTargetOnDragEnter;
                        element.DragLeave -= DropTargetOnDragLeave;
                        element.DragOver -= DropTargetOnDragOver;
                        element.Drop -= DropTargetOnDrop;
                        element.GiveFeedback -= DropTargetOnGiveFeedback;
                    }
                    else
                    {
                        // issue #85: try using preview events for all other elements than ItemsControls
                        element.PreviewDragEnter -= DropTargetOnPreviewDragEnter;
                        element.PreviewDragLeave -= DropTargetOnDragLeave;
                        element.PreviewDragOver -= DropTargetOnPreviewDragOver;
                        element.PreviewDrop -= DropTargetOnPreviewDrop;
                        element.PreviewGiveFeedback -= DropTargetOnGiveFeedback;
                    }
                    break;

                case EventType.Tunneled:
                    element.PreviewDragEnter -= DropTargetOnPreviewDragEnter;
                    element.PreviewDragLeave -= DropTargetOnDragLeave;
                    element.PreviewDragOver -= DropTargetOnPreviewDragOver;
                    element.PreviewDrop -= DropTargetOnPreviewDrop;
                    element.PreviewGiveFeedback -= DropTargetOnGiveFeedback;
                    break;

                case EventType.Bubbled:
                    element.DragEnter -= DropTargetOnDragEnter;
                    element.DragLeave -= DropTargetOnDragLeave;
                    element.DragOver -= DropTargetOnDragOver;
                    element.Drop -= DropTargetOnDrop;
                    element.GiveFeedback -= DropTargetOnGiveFeedback;
                    break;

                case EventType.TunneledBubbled:
                    element.PreviewDragEnter -= DropTargetOnPreviewDragEnter;
                    element.PreviewDragLeave -= DropTargetOnDragLeave;
                    element.PreviewDragOver -= DropTargetOnPreviewDragOver;
                    element.PreviewDrop -= DropTargetOnPreviewDrop;
                    element.PreviewGiveFeedback -= DropTargetOnGiveFeedback;
                    element.DragEnter -= DropTargetOnDragEnter;
                    element.DragLeave -= DropTargetOnDragLeave;
                    element.DragOver -= DropTargetOnDragOver;
                    element.Drop -= DropTargetOnDrop;
                    element.GiveFeedback -= DropTargetOnGiveFeedback;
                    break;

                default:
                    throw new ArgumentException("Unknown value for eventType: " + eventType, nameof(eventType));
            }
        }

        /// <summary>
        /// The register drag drop events.
        /// </summary>
        /// <param name="element">
        /// The element.
        /// </param>
        /// <param name="eventType">
        /// The event type.
        /// </param>
        private static void RegisterDragDropEvents(UIElement element, EventType eventType)
        {
            switch (eventType)
            {
                case EventType.Auto:
                    if (element is ItemsControl)
                    {
                        // use normal events for ItemsControls
                        element.DragEnter += DropTargetOnDragEnter;
                        element.DragLeave += DropTargetOnDragLeave;
                        element.DragOver += DropTargetOnDragOver;
                        element.Drop += DropTargetOnDrop;
                        element.GiveFeedback += DropTargetOnGiveFeedback;
                    }
                    else
                    {
                        // issue #85: try using preview events for all other elements than ItemsControls
                        element.PreviewDragEnter += DropTargetOnPreviewDragEnter;
                        element.PreviewDragLeave += DropTargetOnDragLeave;
                        element.PreviewDragOver += DropTargetOnPreviewDragOver;
                        element.PreviewDrop += DropTargetOnPreviewDrop;
                        element.PreviewGiveFeedback += DropTargetOnGiveFeedback;
                    }
                    break;

                case EventType.Tunneled:
                    element.PreviewDragEnter += DropTargetOnPreviewDragEnter;
                    element.PreviewDragLeave += DropTargetOnDragLeave;
                    element.PreviewDragOver += DropTargetOnPreviewDragOver;
                    element.PreviewDrop += DropTargetOnPreviewDrop;
                    element.PreviewGiveFeedback += DropTargetOnGiveFeedback;
                    break;

                case EventType.Bubbled:
                    element.DragEnter += DropTargetOnDragEnter;
                    element.DragLeave += DropTargetOnDragLeave;
                    element.DragOver += DropTargetOnDragOver;
                    element.Drop += DropTargetOnDrop;
                    element.GiveFeedback += DropTargetOnGiveFeedback;
                    break;

                case EventType.TunneledBubbled:
                    element.PreviewDragEnter += DropTargetOnPreviewDragEnter;
                    element.PreviewDragLeave += DropTargetOnDragLeave;
                    element.PreviewDragOver += DropTargetOnPreviewDragOver;
                    element.PreviewDrop += DropTargetOnPreviewDrop;
                    element.PreviewGiveFeedback += DropTargetOnGiveFeedback;
                    element.DragEnter += DropTargetOnDragEnter;
                    element.DragLeave += DropTargetOnDragLeave;
                    element.DragOver += DropTargetOnDragOver;
                    element.Drop += DropTargetOnDrop;
                    element.GiveFeedback += DropTargetOnGiveFeedback;
                    break;

                default:
                    throw new ArgumentException("Unknown value for eventType: " + eventType.ToString(), nameof(eventType));
            }
        }

        #endregion

        /// <summary>
        /// The drop event type changed.
        /// </summary>
        /// <param name="d">
        /// The d.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void DropEventTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var uiElement = (UIElement)d;

            if (!GetIsDropTarget(uiElement))
                return;

            UnregisterDragDropEvents(uiElement, (EventType)e.OldValue);
            RegisterDragDropEvents(uiElement, (EventType)e.NewValue);
        }

        /// <summary>
        /// The is drop target changed.
        /// </summary>
        /// <param name="d">
        /// The d.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void IsDropTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var uiElement = (UIElement)d;

            var myAdornerLayer = AdornerLayer.GetAdornerLayer(uiElement ?? throw new InvalidOperationException());

            if (myAdornerLayer == null)
            {
                CreateAdornerLayer(uiElement);
            }

            if ((bool)e.NewValue)
            {
                uiElement.AllowDrop = true;

                RegisterDragDropEvents(uiElement, GetDropEventType(d));
            }
            else
            {
                uiElement.AllowDrop = false;

                UnregisterDragDropEvents(uiElement, GetDropEventType(d));

                Mouse.OverrideCursor = null;
            }
        }

        /// <summary>
        /// The create adorner layer.
        /// </summary>
        /// <param name="element">
        ///     The element.
        /// </param>
        private static void CreateAdornerLayer(UIElement element)
        {
            var listBox = element as ListView;
            var ad = new AdornerDecorator();

            if (listBox?.Parent is Grid parent)
            {
                parent.Children.Remove(listBox);
                ad.Child = listBox;
                parent.Children.Add(ad);
            }
        }

        private static void CreateDragAdorner(DropInfo dropInfo)
        {
            var dragInfo = dropInfo.DragInfo;
            var template = GetDropAdornerTemplate(dropInfo.VisualTarget) ?? GetDragAdornerTemplate(dragInfo.VisualSource);
            var templateSelector = GetDropAdornerTemplateSelector(dropInfo.VisualTarget) ?? GetDragAdornerTemplateSelector(dragInfo.VisualSource);

            UIElement adornment = null;

            var useDefaultDragAdorner = template == null && templateSelector == null && GetUseDefaultDragAdorner(dragInfo.VisualSource);
            var useVisualSourceItemSizeForDragAdorner = GetUseVisualSourceItemSizeForDragAdorner(dragInfo.VisualSource);

            if (useDefaultDragAdorner)
            {
                template = dragInfo.VisualSourceItem.GetCaptureScreenDataTemplate(dragInfo.VisualSourceFlowDirection);
            }

            if (template != null || templateSelector != null)
            {
                if (dragInfo.Data is IEnumerable && !(dragInfo.Data is string))
                {
                    if (!useDefaultDragAdorner && ((IEnumerable)dragInfo.Data).Cast<object>().Count() <= 10)
                    {
                        var itemsControl = new ItemsControl();
                        itemsControl.ItemsSource = (IEnumerable)dragInfo.Data;
                        itemsControl.ItemTemplate = template;
                        itemsControl.ItemTemplateSelector = templateSelector;
                        itemsControl.Tag = dragInfo;

                        if (useVisualSourceItemSizeForDragAdorner)
                        {
                            var bounds = VisualTreeHelper.GetDescendantBounds(dragInfo.VisualSourceItem);
                            itemsControl.SetValue(FrameworkElement.MinWidthProperty, bounds.Width);
                        }

                        // The ItemsControl doesn't display unless we create a grid to contain it.
                        // Not quite sure why we need this...
                        var grid = new Grid();
                        grid.Children.Add(itemsControl);
                        adornment = grid;
                    }
                }
                else
                {
                    var contentPresenter = new ContentPresenter();
                    contentPresenter.Content = dragInfo.Data;
                    contentPresenter.ContentTemplate = template;
                    contentPresenter.ContentTemplateSelector = templateSelector;
                    contentPresenter.Tag = dragInfo;

                    if (useVisualSourceItemSizeForDragAdorner)
                    {
                        var bounds = VisualTreeHelper.GetDescendantBounds(dragInfo.VisualSourceItem);
                        contentPresenter.SetValue(FrameworkElement.MinWidthProperty, bounds.Width);
                        contentPresenter.SetValue(FrameworkElement.MinHeightProperty, bounds.Height);
                    }

                    adornment = contentPresenter;
                }
            }

            if (adornment != null)
            {
                if (useDefaultDragAdorner)
                {
                    adornment.Opacity = GetDefaultDragAdornerOpacity(dragInfo.VisualSource);
                }

                var rootElement = RootElementFinder.FindRoot(dropInfo.VisualTarget ?? dragInfo.VisualSource);
                DragAdorner = new DragAdorner(rootElement, adornment, GetDragAdornerTranslation(dragInfo.VisualSource));
            }
        }

        /// <summary>
        /// The drop target on drag enter.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void DropTargetOnDragEnter(object sender, DragEventArgs e)
        {
            DropTargetOnDragOver(sender, e, EventType.Bubbled);
        }

        /// <summary>
        /// The drop target on preview drag enter.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void DropTargetOnPreviewDragEnter(object sender, DragEventArgs e)
        {
            DropTargetOnDragOver(sender, e, EventType.Tunneled);
        }

        /// <summary>
        /// The drop target on drag leave.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void DropTargetOnDragLeave(object sender, DragEventArgs e)
        {
            var listBox = sender as ListBox;
            var myAdornerLayer = AdornerLayer.GetAdornerLayer(listBox ?? throw new InvalidOperationException());
        }

        /// <summary>
        /// The drop target on drag over.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void DropTargetOnDragOver(object sender, DragEventArgs e)
        {
            DropTargetOnDragOver(sender, e, EventType.Bubbled);
        }

        /// <summary>
        /// The drop target on preview drag over.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void DropTargetOnPreviewDragOver(object sender, DragEventArgs e)
        {
            DropTargetOnDragOver(sender, e, EventType.Tunneled);
        }

        /// <summary>
        /// The drop target on drag over.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        /// <param name="eventType">
        /// The event type.
        /// </param>
        private static void DropTargetOnDragOver(object sender, DragEventArgs e, EventType eventType)
        {
            Point adornerPos;
            Size adornerSize;

            var dragInfo = m_DragInfo;
            var dropInfo = new DropInfo(sender, e, dragInfo, eventType);
            var elementPosition = e.GetPosition((IInputElement)sender);

            var listBox = sender as ListBox;
            //var dragInfo = new DragInfo();

            //var myAdornerLayer = AdornerLayer.GetAdornerLayer(listBox ?? throw new InvalidOperationException());
            //var highlightAdorner = new DropTargetHighlightAdorner(myAdornerLayer);
            //myAdornerLayer.Add(highlightAdorner);

            if (DragAdorner == null && dragInfo != null)
            {
                CreateDragAdorner(dropInfo);
            }

            if (DragAdorner != null)
            {
                var tempAdornerPos = e.GetPosition(DragAdorner.AdornedElement);

                if (tempAdornerPos.X >= 0 && tempAdornerPos.Y >= 0)
                {
                    adornerPos = tempAdornerPos;
                }

                // Fixed the flickering adorner - Size changes to zero 'randomly'...?
                if (DragAdorner.RenderSize.Width > 0 && DragAdorner.RenderSize.Height > 0)
                {
                    adornerSize = DragAdorner.RenderSize;
                }

                    // move the adorner
                    var offsetX = adornerSize.Width * -GetDragMouseAnchorPoint(listBox).X;
                    var offsetY = adornerSize.Height * -GetDragMouseAnchorPoint(listBox).Y;
                    adornerPos.Offset(offsetX, offsetY);
                    var maxAdornerPosX = DragAdorner.AdornedElement.RenderSize.Width;
                    var adornerPosRightX = (adornerPos.X + adornerSize.Width);
                    if (adornerPosRightX > maxAdornerPosX)
                    {
                        adornerPos.Offset(-adornerPosRightX + maxAdornerPosX, 0);
                    }
                    if (adornerPos.Y < 0)
                    {
                        adornerPos.Y = 0;
                    }

                DragAdorner.MousePosition = adornerPos;
                DragAdorner.InvalidateVisual();
            }
        }

        /// <summary>
        /// The drop target on drop.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void DropTargetOnDrop(object sender, DragEventArgs e)
        {
            DropTargetOnDrop(sender, e, EventType.Bubbled);
        }

        /// <summary>
        /// The drop target on preview drop.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void DropTargetOnPreviewDrop(object sender, DragEventArgs e)
        {
            DropTargetOnDrop(sender, e, EventType.Tunneled);
        }

        /// <summary>
        /// The drop target on drop.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        /// <param name="eventType">
        /// The event type.
        /// </param>
        private static void DropTargetOnDrop(object sender, DragEventArgs e, EventType eventType)
        {
            var elementPosition = e.GetPosition((IInputElement)sender);
            
            // var listBox = sender as ListBox;
            // var dragInfo = new DragInfo();
            // var myAdornerLayer = AdornerLayer.GetAdornerLayer(listBox ?? throw new InvalidOperationException());
            // myAdornerLayer?.Add(new DropTargetHighlightAdorner(myAdornerLayer, new Behaviors.DropInfo(sender, e, dragInfo, EventType.Auto)));
        }

        /// <summary>
        /// The drop target on give feedback.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void DropTargetOnGiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            if (EffectAdorner != null)
            {
                e.UseDefaultCursors = false;
                e.Handled = true;
                if (Mouse.OverrideCursor != Cursors.Arrow)
                {
                    Mouse.OverrideCursor = Cursors.Cross;
                }
            }
            else
            {
                e.UseDefaultCursors = true;
                e.Handled = true;
                if (Mouse.OverrideCursor != null)
                {
                    Mouse.OverrideCursor = null;
                }
            }
        }
    }
}
