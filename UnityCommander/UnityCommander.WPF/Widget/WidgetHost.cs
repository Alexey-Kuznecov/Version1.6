
using System.Windows;
using System.Windows.Controls;

namespace UnityCommander.WPF.Widget
{
    public sealed class WidgetHost : ItemsControl
    {
        static WidgetHost()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(WidgetHost),
                new FrameworkPropertyMetadata(typeof(WidgetHost)));
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(
                nameof(Title),
                typeof(string),
                typeof(WidgetHost),
                new PropertyMetadata(string.Empty));

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register(
                nameof(IsExpanded),
                typeof(bool),
                typeof(WidgetHost),
                new PropertyMetadata(true));

        public bool IsExpanded
        {
            get => (bool)GetValue(IsExpandedProperty);
            set => SetValue(IsExpandedProperty, value);
        }
    }
}
