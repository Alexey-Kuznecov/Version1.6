using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UnityCommander.Common.Models;

namespace UnityCommander.Core.Behaviors
{
    public static class SortcutBinding
    {
        #region IsSortcutEnabledProperty

        /// <summary>
        /// Gets or Sets whether the control can be used as drag source.
        /// </summary>
        public static readonly DependencyProperty IsDragSourceProperty
            = DependencyProperty.RegisterAttached("IsDragSource",
                                                  typeof(bool),
                                                  typeof(SortcutBinding),
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

        ///// <summary>
        ///// Gets or Sets whether the control can be used as drag source.
        ///// </summary>
        //public static readonly DependencyProperty SortcutProperty
        //    = DependencyProperty.RegisterAttached("Sortcut",
        //                                          typeof(GlobalCommand),
        //                                          typeof(SortcutBinding),
        //                                          new UIPropertyMetadata(false, SortcutChanged));

        ///// <summary>
        ///// Gets whether the control can be used as drag source.
        ///// </summary>
        //public static bool GetSortcut(UIElement target)
        //{
        //    return (bool)target.GetValue(SortcutProperty);
        //}

        ///// <summary>
        ///// Sets whether the control can be used as drag source.
        ///// </summary>
        //public static void SetSortcut(UIElement target, bool value)
        //{
        //    target.SetValue(SortcutProperty, value);
        //}

        //private static void SortcutChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    Grid grid = d as Grid;
        //    InputGesture inputGesture = new KeyGesture(Key.R, ModifierKeys.Control);
        //    KeyBinding binding = new KeyBinding();
        //    InputBinding input = new InputBinding(new DelegateCommand(Command1), inputGesture);
        //    grid.InputBindings.Add(input);
        //}

        public static void Command1()
        {
        }
    }
}
