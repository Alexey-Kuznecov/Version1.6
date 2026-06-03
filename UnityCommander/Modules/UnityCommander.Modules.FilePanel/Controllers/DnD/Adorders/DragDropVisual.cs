
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace UnityCommander.Modules.FilePanel.Controllers.DnD.Adorders
{
    public static class DragDropVisual
    {
        public static readonly DependencyProperty IsDropTargetProperty =
            DependencyProperty.RegisterAttached(
                "IsDropTarget",
                typeof(bool),
                typeof(DragDropVisual),
                 new PropertyMetadata(false, OnChanged));

        public static void SetIsDropTarget(
            DependencyObject element,
            bool value)
        {
            element.SetValue(IsDropTargetProperty, value);
        }

        public static bool GetIsDropTarget(
            DependencyObject element)
        {
            return (bool)element.GetValue(IsDropTargetProperty);
        }

        private static void OnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //if (d is Border border)
            //{
            //    bool value = (bool)e.NewValue;

            //    border.BorderBrush = value ? Brushes.Red : Brushes.DodgerBlue;
            //    border.BorderThickness = value ? new Thickness(3) : new Thickness(0);
            //}
        }
    }
}
