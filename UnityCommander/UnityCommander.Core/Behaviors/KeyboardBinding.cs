using Prism.Commands;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UnityCommander.Integration.Commands;

namespace UnityCommander.Core.Behaviors
{
    public static class KeyboardBinding
    {
        #region IsSortcutEnabledProperty

        /// <summary>
        /// Gets or Sets whether the control can be used as drag source.
        /// </summary>
        public static readonly DependencyProperty IsDragSourceProperty
            = DependencyProperty.RegisterAttached("IsDragSource",
                                                  typeof(bool),
                                                  typeof(KeyboardBinding),
                                                  new UIPropertyMetadata(false, IsDragSourceChanged));

        /// <summary>
        /// Gets whether the control can be used as drag source.
        /// </summary>
        public static bool GetIsDragSource(UIElement target)
        {
            return (bool)target.GetValue(IsDragSourceProperty);
        }

        /// <summary>
        /// Sets whether the control can be used as drag source.
        /// </summary>
        public static void SetIsDragSource(UIElement target, bool value)
        {
            target.SetValue(IsDragSourceProperty, value);
        }

        private static void IsDragSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Grid grid = d as Grid;
            InputGesture inputGesture = new KeyGesture(Key.R, ModifierKeys.Control);
            KeyBinding binding = new KeyBinding();
            InputBinding input = new InputBinding(new DelegateCommand(Command1), inputGesture);
            grid.InputBindings.Add(input);
        }

        #endregion

        /// <summary>
        /// Gets or Sets whether the control can be used as drag source.
        /// </summary>
        public static readonly DependencyProperty KeyProperty
            = DependencyProperty.RegisterAttached("Key",
                                                  typeof(GlobalCommand),
                                                  typeof(KeyboardBinding),
                                                  new UIPropertyMetadata(false, KeyChanged));

        /// <summary>
        /// Gets whether the control can be used as drag source.
        /// </summary>
        public static bool GetKey(UIElement target)
        {
            return (bool)target.GetValue(KeyProperty);
        }

        /// <summary>
        /// Sets whether the control can be used as drag source.
        /// </summary>
        public static void SetKey(UIElement target, bool value)
        {
            target.SetValue(KeyProperty, value);
        }

        private static void KeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Grid grid = d as Grid;
            InputGesture inputGesture = new KeyGesture(Key.R, ModifierKeys.Control);
            KeyBinding binding = new KeyBinding();
            InputBinding input = new InputBinding(new DelegateCommand(Command1), inputGesture);
            grid.InputBindings.Add(input);
        }

        public static void Command1()
        {
        }
    }
}
