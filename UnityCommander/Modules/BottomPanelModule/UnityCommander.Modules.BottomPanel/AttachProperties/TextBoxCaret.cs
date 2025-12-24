using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace UnityCommander.Modules.BottomPanel.AttachProperties
{
    public static class TextBoxCaret
    {
        public static readonly DependencyProperty CaretIndexProperty =
            DependencyProperty.RegisterAttached(
                "CaretIndex",
                typeof(int),
                typeof(TextBoxCaret),
                new PropertyMetadata(-1, OnCaretIndexChanged));

        public static int GetCaretIndex(DependencyObject obj)
            => (int)obj.GetValue(CaretIndexProperty);

        public static void SetCaretIndex(DependencyObject obj, int value)
            => obj.SetValue(CaretIndexProperty, value);

        private static void OnCaretIndexChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox tb && e.NewValue is int index && index >= 0)
            {
                // 🔥 делаем через Dispatcher, чтобы гарантированно после обновления Text
                tb.Dispatcher.BeginInvoke(() =>
                {
                    tb.CaretIndex = Math.Min(index, tb.Text.Length);
                }, DispatcherPriority.Render);
            }
        }
    }
}
