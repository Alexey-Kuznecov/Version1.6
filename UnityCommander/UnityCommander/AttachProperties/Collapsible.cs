
using System.Windows;
using System.Windows.Controls;

namespace UnityCommander.AttachProperties
{
    public static class Collapsible
    {
        public static bool GetIsCollapsed(DependencyObject obj) =>
            (bool)obj.GetValue(IsCollapsedProperty);

        public static void SetIsCollapsed(DependencyObject obj, bool value) =>
            obj.SetValue(IsCollapsedProperty, value);

        public static readonly DependencyProperty IsCollapsedProperty =
            DependencyProperty.RegisterAttached(
                "IsCollapsed",
                typeof(bool),
                typeof(Collapsible),
                new PropertyMetadata(false, OnChanged));

        private static void OnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is RowDefinition row)
                row.Height = (bool)e.NewValue ? new GridLength(0) : GridLength.Auto;
        }
    }
}
