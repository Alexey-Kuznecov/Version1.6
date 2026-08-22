
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using UnityCommander.Modules.FilePanel.Services;

namespace UnityCommander.Modules.FilePanel.Behaviors
{
    public static class ScrollBehavior
    {
        public static readonly DependencyProperty EnableProperty =
            DependencyProperty.RegisterAttached(
                "Enable",
                typeof(bool),
                typeof(ScrollBehavior),
                new PropertyMetadata(false, OnEnableChanged));

        public static void SetEnable(DependencyObject obj, bool value)
            => obj.SetValue(EnableProperty, value);


        public static readonly DependencyProperty ViewportTargetProperty =
            DependencyProperty.RegisterAttached(
                "ViewportTarget",
                typeof(object),
                typeof(ScrollBehavior),
                new PropertyMetadata(null));

        public static void SetViewportTarget(DependencyObject obj, IViewportHost value)
            => obj.SetValue(ViewportTargetProperty, value);

        public static IViewportHost GetViewportTarget(DependencyObject obj)
            => (IViewportHost)obj.GetValue(ViewportTargetProperty);

        private static void OnEnableChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            if (d is not ListView listView)
                return;

            listView.Loaded += (_, __) =>
            {
                var scrollViewer = GetScrollViewer(listView);
                var target = GetViewportTarget(listView);

                if (scrollViewer == null || target == null)
                    return;

                void UpdateViewport()
                {
                    target.Mapper.Update(
                        scrollViewer.VerticalOffset,
                        scrollViewer.ViewportHeight);

                    //Debug.WriteLine(
                    //    $"Viewport: {scrollViewer.ViewportHeight}, " +
                    //    $"Offset: {scrollViewer.VerticalOffset}, " +
                    //    $"ActualHeight: {scrollViewer.ActualHeight}");
                }

                scrollViewer.ScrollChanged += (_, __) =>
                {
                    UpdateViewport();
                };

                listView.Dispatcher.BeginInvoke(
                    UpdateViewport,
                    DispatcherPriority.Loaded);
            };
        }

        public static ScrollViewer GetScrollViewer(DependencyObject obj)
        {
            if (obj is ScrollViewer sv)
                return sv;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                var child = VisualTreeHelper.GetChild(obj, i);
                var result = GetScrollViewer(child);
                if (result != null)
                    return result;
            }

            return null;
        }

        public static event Action<int, int>? ScrollChanged;
    }
}
