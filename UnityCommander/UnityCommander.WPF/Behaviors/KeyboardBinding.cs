
using System.Windows;
using System.Windows.Input;
using UnityCommander.Abstractions.Keyboard;

namespace UnityCommander.WPF.Behaviors
{
    public static class KeyboardBinding
    {
        private static IShortcutContextService? _context;

        public static readonly DependencyProperty ScopeProperty =
            DependencyProperty.RegisterAttached(
                "Scope",
                typeof(ShortcutScope),
                typeof(KeyboardBinding),
                new PropertyMetadata(default(ShortcutScope), OnScopeChanged));

        public static ShortcutScope GetScope(DependencyObject obj)
        {
            return (ShortcutScope)obj.GetValue(ScopeProperty);
        }

        public static void SetScope(DependencyObject obj, ShortcutScope value)
        {
            obj.SetValue(ScopeProperty, value);
        }

        private static void OnScopeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not FrameworkElement element)
                return;

            element.Focusable = true;

            element.AddHandler(
                UIElement.GotKeyboardFocusEvent,
                new KeyboardFocusChangedEventHandler(OnGotKeyboardFocus),
                true);

            element.AddHandler(
                UIElement.LostKeyboardFocusEvent,
                new KeyboardFocusChangedEventHandler(OnLostKeyboardFocus),
                true);
        }

        private static void OnGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var element = (FrameworkElement)sender;

            var scope = GetScope(element);

            _context?.Push(element, scope);
        }

        private static void OnLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var element = (FrameworkElement)sender;

            _context?.Pop(element);
        }

        public static void Initialize(IShortcutContextService context)
        {
            _context = context;
        }
    }
}
