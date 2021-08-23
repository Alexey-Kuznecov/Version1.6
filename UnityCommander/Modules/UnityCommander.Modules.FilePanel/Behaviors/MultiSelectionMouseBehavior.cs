
namespace UnityCommander.Modules.FilePanel.Behaviors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;

    using UnityCommander.Common.Models.Directory;

    /// <summary>
    /// The multi selection by mouse behavior.
    /// </summary>
    public class MultiSelectionMouseBehavior
    {
        /// <summary>
        /// The is enable selection property.
        /// </summary>
        public static readonly DependencyProperty IsEnableSelectionProperty = DependencyProperty.RegisterAttached(
            "IsEnableSelection", typeof(bool), typeof(MultiSelectionMouseBehavior), new UIPropertyMetadata(false, IsEnableSelectionChanged));

        /// <summary>
        /// Gets or sets the drag start point.
        /// </summary>
        public static Point? DragStartPoint { get; set; }

        /// <summary>
        /// Gets or sets the somewhere in your main window class.
        /// </summary>
        public static Dictionary<UIElement, MultiSelectionMouseAdorner> MainAdornerCollection { get; set; } = new ();

        /// <summary>
        /// The set is enable selection mouse.
        /// </summary>
        /// <param name="element">
        /// The element.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public static void SetIsEnableSelection(DependencyObject element, bool value)
        {
            element.SetValue(IsEnableSelectionProperty, value);
        }

        /// <summary>
        /// The get is enable selection mouse.
        /// </summary>
        /// <param name="element">
        /// The element.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool GetIsEnableSelection(DependencyObject element)
        {
            return (bool)element.GetValue(IsEnableSelectionProperty);
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
        private static void IsEnableSelectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                var uiElement = (UIElement)d;
                var myAdornerLayer = AdornerLayer.GetAdornerLayer(uiElement ?? throw new InvalidOperationException());
                MainAdornerCollection.Add(uiElement, new MultiSelectionMouseAdorner(uiElement));
                if (myAdornerLayer == null)
                {
                    CreateAdornerLayer(uiElement);
                    myAdornerLayer = AdornerLayer.GetAdornerLayer(uiElement);
                    myAdornerLayer?.Add(MainAdornerCollection.Single(l => l.Key.Equals(uiElement)).Value);
                }

                if ((bool)e.NewValue)
                {
                    uiElement.PreviewMouseLeftButtonDown += DragSourceOnMouseLeftButtonDown;
                    uiElement.PreviewMouseLeftButtonUp += DragSourceOnMouseLeftButtonUp;
                    uiElement.PreviewMouseMove += DragSourceOnMouseMove;
                    uiElement.MouseLeave += DragSourceOnMouseMouseLeave;
                }
                else
                {
                    uiElement.PreviewMouseLeftButtonDown -= DragSourceOnMouseLeftButtonDown;
                    uiElement.PreviewMouseLeftButtonUp -= DragSourceOnMouseLeftButtonUp;
                    uiElement.PreviewMouseMove -= DragSourceOnMouseMove;
                }
            }
        }

        /// <summary>
        /// The drag source on mouse mouse leave.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void DragSourceOnMouseMouseLeave(object sender, MouseEventArgs e)
        {
            DragStartPoint = null;
            var adorner = GetMultiSelectionMouseAdorner((UIElement)sender);
            adorner.HighlightArea = new Rect();
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
            if (e.ChangedButton == MouseButton.Left)
            {
                DragStartPoint = null;
                var adorner = GetMultiSelectionMouseAdorner((UIElement)sender);
                adorner.HighlightArea = new Rect();
            }
        }

        /// <summary>
        /// The clear highlight area.
        /// </summary>
        /// <param name="uiElement">
        /// The element.
        /// </param>
        /// <returns>
        /// The <see cref="MultiSelectionMouseAdorner"/>.
        /// </returns>
        private static MultiSelectionMouseAdorner GetMultiSelectionMouseAdorner(UIElement uiElement) =>
            MainAdornerCollection.Single(e => e.Key.Equals(uiElement)).Value;

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
            if (DragStartPoint.HasValue)
            {
                if (sender is ListView listView)
                {
                    Rect rect = new Rect(DragStartPoint.Value, e.GetPosition(listView) - DragStartPoint.Value);
                    var adorner = GetMultiSelectionMouseAdorner((UIElement)sender);
                    adorner.HighlightArea = rect;
                    var items = GetItemAt<BaseDirectory>(listView, rect);
                    if (items.Count > 0)
                    {
                        listView.SelectedItems.Clear();
                        foreach (var i in items)
                        {
                            listView.SelectedItems.Add(i);
                        }
                    }
                    else
                    {
                        listView.SelectedItems.Clear();
                    }
                }
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
            if (e.ChangedButton == MouseButton.Left)
            {
                var listBox = sender as ListBox;
                DragStartPoint = e.GetPosition((IInputElement)sender);
                listBox?.SelectedItems.Clear();
            }
        }


        /// <summary>
        /// The get item at.
        /// </summary>
        /// <param name="listBox">
        /// The listbox.
        /// </param>
        /// <param name="areaOfInterest">
        /// The area of interest.
        /// </param>
        /// <typeparam name="T">
        /// The area of interest.
        /// </typeparam>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        private static List<T> GetItemAt<T>(ListBox listBox, Rect areaOfInterest)
        {
            var list = new List<T>();
            var rect = new RectangleGeometry(areaOfInterest);
            var hitTestParams = new GeometryHitTestParameters(rect);
            var resultCallback = new HitTestResultCallback(x => HitTestResultBehavior.Continue);
            var filterCallback = new HitTestFilterCallback(x =>
                {
                    if (x is ListBoxItem)
                    {
                        var item = (T)((ListBoxItem)x).Content;
                        list.Add(item);
                    }

                    return HitTestFilterBehavior.Continue;
                });

            VisualTreeHelper.HitTest(listBox, filterCallback, resultCallback, hitTestParams);
            return list;
        }

        /// <summary>
        /// The create adorner layer.
        /// </summary>
        /// <param name="element">
        /// The element.
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
    }
}
