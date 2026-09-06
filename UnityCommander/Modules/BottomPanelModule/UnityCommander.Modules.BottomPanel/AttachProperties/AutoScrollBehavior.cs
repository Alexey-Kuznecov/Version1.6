
using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace UnityCommander.Modules.BottomPanel.AttachProperties
{
    public static class ConsoleAutoScrollBehavior
    {
        public static readonly DependencyProperty EnableProperty =
            DependencyProperty.RegisterAttached(
                "Enable",
                typeof(bool),
                typeof(ConsoleAutoScrollBehavior),
                new PropertyMetadata(false, OnEnableChanged));

        private static readonly DependencyProperty HandlerProperty =
            DependencyProperty.RegisterAttached(
                "Handler",
                typeof(NotifyCollectionChangedEventHandler),
                typeof(ConsoleAutoScrollBehavior));

        private const double BottomThreshold = 10;

        public static void SetEnable(DependencyObject element, bool value)
            => element.SetValue(EnableProperty, value);

        public static bool GetEnable(DependencyObject element)
            => (bool)element.GetValue(EnableProperty);

        private static void OnEnableChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            if (d is not ScrollViewer scrollViewer)
                return;

            if ((bool)e.NewValue)
            {
                scrollViewer.Loaded += OnLoaded;
                scrollViewer.Unloaded += OnUnloaded;
            }
            else
            {
                scrollViewer.Loaded -= OnLoaded;
                scrollViewer.Unloaded -= OnUnloaded;
            }
        }

        private static void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (sender is not ScrollViewer scrollViewer)
                return;

            if (scrollViewer.Content is not ItemsControl itemsControl)
                return;

            if (itemsControl.ItemsSource is not INotifyCollectionChanged collection)
                return;

            NotifyCollectionChangedEventHandler handler = (_, args) =>
            {
                if (args.Action != NotifyCollectionChangedAction.Add &&
                    args.Action != NotifyCollectionChangedAction.Reset)
                {
                    return;
                }

                var wasAtBottom =
                    scrollViewer.VerticalOffset +
                    scrollViewer.ViewportHeight >=
                    scrollViewer.ExtentHeight - BottomThreshold;

                if (!wasAtBottom)
                    return;

                scrollViewer.Dispatcher.BeginInvoke(
                    DispatcherPriority.Loaded,
                    new Action(() =>
                    {
                        scrollViewer.ScrollToEnd();
                    }));
            };

            scrollViewer.SetValue(HandlerProperty, handler);
            collection.CollectionChanged += handler;
        }

        private static void OnUnloaded(object sender, RoutedEventArgs e)
        {
            if (sender is not ScrollViewer scrollViewer)
                return;

            if (scrollViewer.Content is not ItemsControl itemsControl)
                return;

            if (itemsControl.ItemsSource is not INotifyCollectionChanged collection)
                return;

            var handler =
                (NotifyCollectionChangedEventHandler?)
                scrollViewer.GetValue(HandlerProperty);

            if (handler != null)
                collection.CollectionChanged -= handler;

            scrollViewer.ClearValue(HandlerProperty);
        }
    }
}
