
using AvalonDock.Controls;
using System.Windows;
using Prism.Ioc;
using DragDrop = UnityCommander.WPF.DragDrop.DragDrop;

namespace UnityCommander.Modules.FilePanel.Controllers.DnD
{

    public static class AvalonDockDragDrop
    {
        public static readonly DependencyProperty EnableProperty =
            DependencyProperty.RegisterAttached(
                "Enable",
                typeof(bool),
                typeof(AvalonDockDragDrop),
                new PropertyMetadata(false, OnEnableChanged));

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

        private static void OnEnableChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            if (d is not LayoutDocumentTabItem tab ||
                e.NewValue is not true)
                return;

            var adapter =
                ContainerLocator.Container.Resolve<GongDropAdapter>();

            DragDrop.SetIsDropTarget(tab, true);
            DragDrop.SetDropHandler(tab, adapter);
        }
    }
}
