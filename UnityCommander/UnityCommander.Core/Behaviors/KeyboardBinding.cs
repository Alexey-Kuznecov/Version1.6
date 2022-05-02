using Prism.Commands;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace UnityCommander.Core.Behaviors
{
    public static class KeyboardBinding
    {
        #region IsSortcutEnabledProperty

        /// <summary>
        /// Gets or Sets whether the control can be used as drag source.
        /// </summary>
        public static readonly DependencyProperty IsEnableProperty
            = DependencyProperty.RegisterAttached("IsEnable",
                                                  typeof(bool),
                                                  typeof(KeyboardBinding),
                                                  new UIPropertyMetadata(IsEnableChanged));

        /// <summary>
        /// Gets whether the control can be used as drag source.
        /// </summary>
        public static bool GetIsEnable(UIElement target)
        {
            return (bool)target.GetValue(IsEnableProperty);
        }

        /// <summary>
        /// Sets whether the control can be used as drag source.
        /// </summary>
        public static void SetIsEnable(UIElement target, bool value)
        {
            target.SetValue(IsEnableProperty, value);
        }

        private static void IsEnableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Grid grid = d as Grid;
            var vm = grid?.DataContext as IKeyBinding;
            vm?.SetBinding(d, new KeyboardManager());
        }

        #endregion

        /// <summary>
        /// Gets or Sets whether the control can be used as drag source.
        /// </summary>
        public static readonly DependencyProperty KeyProperty
            = DependencyProperty.RegisterAttached("Key",
                                                  typeof(GlobalCommand),
                                                  typeof(KeyboardBinding),
                                                  new UIPropertyMetadata(null, KeyChanged));

        /// <summary>
        /// Gets whether the control can be used as drag source.
        /// </summary>
        public static GlobalCommand GetKey(UIElement target)
        {
            return (GlobalCommand)target.GetValue(KeyProperty);
        }

        /// <summary>
        /// Sets whether the control can be used as drag source.
        /// </summary>
        public static void SetKey(UIElement target, GlobalCommand value)
        {
            target.SetValue(KeyProperty, value);
        }

        private static void KeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Grid grid = d as Grid;
            InputGesture inputGesture = new KeyGesture(Key.R, ModifierKeys.Control);
            KeyBinding binding = new KeyBinding();
            InputBinding input = new InputBinding(new DelegateCommand(Command1), inputGesture);
            grid?.InputBindings.Add(input);
        }

        public static void Command1()
        {
        }
    }
}
