
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace UnityCommander.Modules.LeftSideBars.Behaviors
{
    public static class ScrollViewerWheelBehavior
    {
        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.RegisterAttached(
                "IsEnabled",
                typeof(bool),
                typeof(ScrollViewerWheelBehavior),
                new PropertyMetadata(false, OnIsEnabledChanged));

        public static void SetIsEnabled(DependencyObject element, bool value)
            => element.SetValue(IsEnabledProperty, value);

        public static bool GetIsEnabled(DependencyObject element)
            => (bool)element.GetValue(IsEnabledProperty);

        private static void OnIsEnabledChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            if (d is not ScrollViewer scrollViewer)
                return;

            if ((bool)e.NewValue)
                scrollViewer.PreviewMouseWheel += OnPreviewMouseWheel;
            else
                scrollViewer.PreviewMouseWheel -= OnPreviewMouseWheel;
        }

        private static void OnPreviewMouseWheel(
            object sender,
            MouseWheelEventArgs e)
        {
            if (sender is not ScrollViewer scrollViewer)
                return;

            if (scrollViewer.ScrollableHeight <= 0)
                return;

            const double scrollStep = 48;

            var offset = scrollViewer.VerticalOffset;

            offset -= Math.Sign(e.Delta) * scrollStep;

            offset = Math.Max(
                0,
                Math.Min(offset, scrollViewer.ScrollableHeight));

            scrollViewer.ScrollToVerticalOffset(offset);

            e.Handled = true;
        }
    }
}
