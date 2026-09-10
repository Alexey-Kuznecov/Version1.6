
using System.Windows;
using System.Windows.Input;
using UnityCommander.WPF.Input;

namespace UnityCommander.WPF.Behaviors
{
    public static class InputCaptureBehavior
    {
        private static IInputCaptureManager? _captureManager;

        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.RegisterAttached(
                "IsEnabled",
                typeof(bool),
                typeof(InputCaptureBehavior),
                new PropertyMetadata(false, OnIsEnabledChanged));

        private static readonly DependencyProperty ContextProperty =
            DependencyProperty.RegisterAttached(
                "Context",
                typeof(IInputContext),
                typeof(InputCaptureBehavior));

        public static void SetIsEnabled(
            DependencyObject element,
            bool value)
        {
            element.SetValue(IsEnabledProperty, value);
        }

        public static bool GetIsEnabled(
            DependencyObject element)
        {
            return (bool)element.GetValue(IsEnabledProperty);
        }

        private static void OnIsEnabledChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            if (d is not UIElement element)
                return;

            if ((bool)e.NewValue)
            {
                element.GotKeyboardFocus += OnGotKeyboardFocus;
                element.LostKeyboardFocus += OnLostKeyboardFocus;
            }
            else
            {
                element.GotKeyboardFocus -= OnGotKeyboardFocus;
                element.LostKeyboardFocus -= OnLostKeyboardFocus;

                PopContext(element);
            }
        }

        private static void OnGotKeyboardFocus(
            object sender,
            KeyboardFocusChangedEventArgs e)
        {
            if (sender is not UIElement element)
                return;

            if (_captureManager is null)
                return;

            if (element.GetValue(ContextProperty) is IInputContext)
                return;

            var context = new TextInputContext();

            element.SetValue(ContextProperty, context);
            _captureManager?.Push(context);
        }

        private static void OnLostKeyboardFocus(
            object sender,
            KeyboardFocusChangedEventArgs e)
        {
            if (sender is UIElement element)
                PopContext(element);
        }

        private static void PopContext(UIElement element)
        {
            var context =
                element.GetValue(ContextProperty) as IInputContext;

            if (context is null)
                return;

            _captureManager?.Pop(context);

            element.ClearValue(ContextProperty);
        }

        public static void Initialize(IInputCaptureManager inputCapture)
        {
            _captureManager = inputCapture;
        }
    }
}
