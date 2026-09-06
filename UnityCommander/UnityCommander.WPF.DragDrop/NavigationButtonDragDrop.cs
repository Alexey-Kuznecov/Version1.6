
using System.Windows;
using System.Windows.Controls;

namespace UnityCommander.WPF.DragDrop
{
    public static class NavigationButtonDragDrop
    {
        public static readonly DependencyProperty EnableProperty =
            DependencyProperty.RegisterAttached(
                "Enable",
                typeof(bool),
                typeof(NavigationButtonDragDrop),
                new PropertyMetadata(false, OnEnableChanged));

        public static readonly DependencyProperty DropPathProperty =
            DependencyProperty.RegisterAttached(
                "DropPath",
                typeof(string),
                typeof(NavigationButtonDragDrop));

        public static void SetEnable(
            DependencyObject element,
            bool value)
        {
            element.SetValue(EnableProperty, value);
        }

        public static bool GetEnable(
            DependencyObject element)
        {
            return (bool)element.GetValue(EnableProperty);
        }

        public static void SetDropPath(
            DependencyObject element,
            string value)
        {
            element.SetValue(DropPathProperty, value);
        }

        public static string GetDropPath(
            DependencyObject element)
        {
            return (string)element.GetValue(DropPathProperty);
        }

        private static void OnEnableChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            if (d is not Button button ||
                e.NewValue is not true)
                return;

            var adapter =
                ContainerLocator.Container.Resolve<GongDropAdapter>();

            DragDrop.SetIsDropTarget(button, true);
            DragDrop.SetDropHandler(button, adapter);
        }
    }
}
