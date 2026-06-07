using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace UnityCommander.Modules.BottomPanel.AttachProperties
{
    public static class TextBoxCaretBinding
    {
        public static readonly DependencyProperty CaretIndexProperty =
            DependencyProperty.RegisterAttached(
                "CaretIndex",
                typeof(int),
                typeof(TextBoxCaretBinding),
                new FrameworkPropertyMetadata(
                    0,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnCaretIndexChanged));

        private static readonly DependencyProperty IsAttachedProperty =
            DependencyProperty.RegisterAttached(
                "IsAttached",
                typeof(bool),
                typeof(TextBoxCaretBinding),
                new PropertyMetadata(false));

        public static int GetCaretIndex(DependencyObject obj)
            => (int)obj.GetValue(CaretIndexProperty);

        public static void SetCaretIndex(DependencyObject obj, int value)
            => obj.SetValue(CaretIndexProperty, value);

        private static bool GetIsAttached(DependencyObject obj)
            => (bool)obj.GetValue(IsAttachedProperty);

        private static void SetIsAttached(DependencyObject obj, bool value)
            => obj.SetValue(IsAttachedProperty, value);

        private static void OnCaretIndexChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            if (d is not TextBox tb)
                return;

            Attach(tb);

            int newIndex = (int)e.NewValue;
            int clamped = Math.Min(newIndex, tb.Text.Length);

            if (tb.CaretIndex != clamped)
            {
                tb.CaretIndex = clamped;
            }
        }

        private static void Attach(TextBox tb)
        {
            if (GetIsAttached(tb))
                return;

            SetIsAttached(tb, true);

            tb.SelectionChanged += OnSelectionChanged;
            tb.TextChanged += OnTextChanged;
        }

        private static void OnSelectionChanged(object sender, RoutedEventArgs e)
        {
            if (sender is not TextBox tb)
                return;

            SyncCaretToBinding(tb);
        }

        private static void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is not TextBox tb)
                return;

            SyncCaretToBinding(tb);
        }

        private static void SyncCaretToBinding(TextBox tb)
        {
            int caret = tb.CaretIndex;
            int bound = GetCaretIndex(tb);

            if (caret != bound)
            {
                SetCaretIndex(tb, caret);
            }
        }
    }
}
