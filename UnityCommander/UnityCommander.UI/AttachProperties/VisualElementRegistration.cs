
using System.Windows;
using UnityCommander.UI.Visual;

namespace UnityCommander.UI.AttachProperties
{
    public static class VisualElementRegistration
    {
        private static IVisualElementRegistry _registry = null!;

        public static void Initialize(IVisualElementRegistry registry)
        {
            _registry = registry;
        }

        public static readonly DependencyProperty ModelProperty =
            DependencyProperty.RegisterAttached(
                "Model",
                typeof(object),
                typeof(VisualElementRegistration),
                new PropertyMetadata(null, OnModelChanged));

        public static void SetModel(DependencyObject element, object? value)
            => element.SetValue(ModelProperty, value);

        public static object? GetModel(DependencyObject element)
            => element.GetValue(ModelProperty);

        private static void OnModelChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            if (d is not FrameworkElement element)
                return;

            if (e.OldValue != null)
            {
                element.Loaded -= OnLoaded;
                element.Unloaded -= OnUnloaded;
            }

            if (e.NewValue != null)
            {
                element.Loaded += OnLoaded;
                element.Unloaded += OnUnloaded;
            }
        }

        private static void OnLoaded(object sender, RoutedEventArgs e)
        {
            var element = (FrameworkElement)sender;
            var model = GetModel(element);

            if (model != null)
                _registry.Register(model, element);
        }

        private static void OnUnloaded(object sender, RoutedEventArgs e)
        {
            var element = (FrameworkElement)sender;
            var model = GetModel(element);

            if (model != null)
                _registry.Unregister(model, element);
        }
    }
}
