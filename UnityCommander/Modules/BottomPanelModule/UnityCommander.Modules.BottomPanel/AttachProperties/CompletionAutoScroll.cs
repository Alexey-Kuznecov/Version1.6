
using System.Windows;
using System.Windows.Controls;

namespace UnityCommander.Modules.BottomPanel.AttachProperties
{
    public static class CompletionAutoScroll
    {
        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.RegisterAttached(
                "SelectedIndex",
                typeof(int),
                typeof(CompletionAutoScroll),
                new PropertyMetadata(-1, OnSelectedIndexChanged));

        public static void SetSelectedIndex(
            DependencyObject element,
            int value)
        {
            element.SetValue(SelectedIndexProperty, value);
        }

        public static int GetSelectedIndex(
            DependencyObject element)
        {
            return (int)element.GetValue(SelectedIndexProperty);
        }

        private static void OnSelectedIndexChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            if (d is not ListBox listBox)
                return;

            var index = (int)e.NewValue;

            if (index < 0 || index >= listBox.Items.Count)
                return;

            listBox.ScrollIntoView(listBox.Items[index]);
        }
    }
}
