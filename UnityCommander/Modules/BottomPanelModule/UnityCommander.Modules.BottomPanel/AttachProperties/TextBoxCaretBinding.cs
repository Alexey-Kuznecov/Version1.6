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

        public static int GetCaretIndex(DependencyObject obj)
            => (int)obj.GetValue(CaretIndexProperty);

        public static void SetCaretIndex(DependencyObject obj, int value)
            => obj.SetValue(CaretIndexProperty, value);

        private static void OnCaretIndexChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            if (d is not TextBox tb)
                return;

            int newIndex = (int)e.NewValue;

            if (tb.CaretIndex != newIndex)
            {
                tb.CaretIndex = Math.Min(newIndex, tb.Text.Length);
            }

            Attach(tb);
        }

        private static void Attach(TextBox tb)
        {
            tb.SelectionChanged -= OnSelectionChanged;
            tb.SelectionChanged += OnSelectionChanged;
        }

        private static void OnSelectionChanged(object sender, RoutedEventArgs e)
        {
            if (sender is not TextBox tb)
                return;

            int caret = tb.CaretIndex;
            int bound = GetCaretIndex(tb);

            if (caret != bound)
            {
                SetCaretIndex(tb, caret);
            }
        }
    }
}
