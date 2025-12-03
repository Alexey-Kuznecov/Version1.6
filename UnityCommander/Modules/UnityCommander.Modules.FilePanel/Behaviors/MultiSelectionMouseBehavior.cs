
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

    public class MultiSelectionMouseBehavior
    {
        public static readonly DependencyProperty IsEnableSelectionProperty =
            DependencyProperty.RegisterAttached(
                "IsEnableSelection",
                typeof(bool),
                typeof(MultiSelectionMouseBehavior),
                new UIPropertyMetadata(false, IsEnableSelectionChanged));

        public static Point? DragStartPoint { get; set; }
        public static Dictionary<UIElement, MultiSelectionMouseAdorner> MainAdornerCollection { get; set; } = new();

        public static void SetIsEnableSelection(DependencyObject element, bool value)
        {
            element.SetValue(IsEnableSelectionProperty, value);
        }

        public static bool GetIsEnableSelection(DependencyObject element)
        {
            return (bool)element.GetValue(IsEnableSelectionProperty);
        }

        private static void IsEnableSelectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not UIElement uiElement) return;

            if ((bool)e.NewValue)
            {
                var adornerLayer = AdornerLayer.GetAdornerLayer(uiElement);
                var adorner = new MultiSelectionMouseAdorner(uiElement);
                MainAdornerCollection[uiElement] = adorner;

                if (adornerLayer != null)
                    adornerLayer.Add(adorner);
                else
                    CreateAdornerLayer(uiElement);

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
                uiElement.MouseLeave -= DragSourceOnMouseMouseLeave;
            }
        }

        private static void DragSourceOnMouseMouseLeave(object sender, MouseEventArgs e)
        {
            DragStartPoint = null;
            GetMultiSelectionMouseAdorner((UIElement)sender).HighlightArea = new Rect();
        }

        private static void DragSourceOnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left) return;

            DragStartPoint = null;
            GetMultiSelectionMouseAdorner((UIElement)sender).HighlightArea = new Rect();
        }

        private static void DragSourceOnMouseMove(object sender, MouseEventArgs e)
        {
            if (!DragStartPoint.HasValue) return;
            if (sender is not ListView listView) return;

            var rect = new Rect(DragStartPoint.Value, e.GetPosition(listView) - DragStartPoint.Value);
            var adorner = GetMultiSelectionMouseAdorner((UIElement)sender);
            adorner.HighlightArea = rect;

            var items = GetItemAt<BaseDirectory>(listView, rect);

            // Ctrl/Shift поддержка
            bool ctrl = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
            bool shift = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);

            if (!ctrl && !shift)
                listView.SelectedItems.Clear();

            foreach (var i in items)
                if (!listView.SelectedItems.Contains(i))
                    listView.SelectedItems.Add(i);
        }

        private static void DragSourceOnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left) return;

            if (sender is not ListView listView) return;

            DragStartPoint = e.GetPosition(listView);

            // Подавляем стандартное выделение ListView
            e.Handled = true;

            bool ctrl = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
            bool shift = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);

            if (!ctrl && !shift)
                listView.SelectedItems.Clear();
        }

        private static List<T> GetItemAt<T>(ListView listView, Rect areaOfInterest)
        {
            var list = new List<T>();
            var rect = new RectangleGeometry(areaOfInterest);
            var hitTestParams = new GeometryHitTestParameters(rect);

            VisualTreeHelper.HitTest(listView,
                new HitTestFilterCallback(x =>
                {
                    if (x is ListViewItem)
                        return HitTestFilterBehavior.Continue;
                    return HitTestFilterBehavior.Continue;
                }),
                new HitTestResultCallback(x =>
                {
                    if (x.VisualHit is ListViewItem item)
                        list.Add((T)item.Content);
                    return HitTestResultBehavior.Continue;
                }),
                hitTestParams);

            return list;
        }

        private static MultiSelectionMouseAdorner GetMultiSelectionMouseAdorner(UIElement element)
            => MainAdornerCollection[element];

        private static void CreateAdornerLayer(UIElement element)
        {
            if (element is not ListView listView) return;
            var ad = new AdornerDecorator();

            if (listView.Parent is Grid parent)
            {
                parent.Children.Remove(listView);
                ad.Child = listView;
                parent.Children.Add(ad);

                var layer = AdornerLayer.GetAdornerLayer(listView);
                if (layer != null)
                    layer.Add(MainAdornerCollection[listView]);
            }
        }
    }

}
