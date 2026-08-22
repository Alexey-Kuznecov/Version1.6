
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace UnityCommander.WPF.Behaviors
{
    public static class CursorTargetBehavior
    {
        private static ICursorTargetService _cursorTargetService;

        public static readonly DependencyProperty EnableProperty =
            DependencyProperty.RegisterAttached(
                "Enable",
                typeof(bool),
                typeof(CursorTargetBehavior),
                new PropertyMetadata(false, OnEnableChanged));
    
       
        public static readonly RoutedEvent TargetChangedEvent =
           EventManager.RegisterRoutedEvent(
               "TargetChanged",
               RoutingStrategy.Bubble,
               typeof(RoutedEventHandler),
               typeof(CursorTargetBehavior));

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
            if (d is not UIElement element)
                return;

            if (e.NewValue is true)
            {
                element.MouseMove += OnMouseMove;
                element.MouseLeave += OnMouseLeave;
            }
            else
            {
                element.MouseMove -= OnMouseMove;
                element.MouseLeave -= OnMouseLeave;
            }

            _cursorTargetService 
                = ContainerLocator.Container.Resolve<ICursorTargetService>();
        }

        private static void OnMouseMove(
            object sender,
            MouseEventArgs e)
        {
            var listView = (ListView)sender;

            _cursorTargetService.Update(
                listView,
                e.GetPosition(listView));
        }

        private static void OnMouseLeave(
            object sender,
            MouseEventArgs e)
        {
            if (sender is not ListView listView)
                return;

            _cursorTargetService.Clear(listView);
        }
    }
}
