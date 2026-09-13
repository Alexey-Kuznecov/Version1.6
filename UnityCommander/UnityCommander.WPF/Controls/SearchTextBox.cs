
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace UnityCommander.WPF.Controls
{
    public class SearchTextBox : TextBox
    {
        static SearchTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(SearchTextBox),
                new FrameworkPropertyMetadata(typeof(SearchTextBox)));
        }

        public static readonly DependencyProperty PlaceholderProperty =
            DependencyProperty.Register(
                nameof(Placeholder),
                typeof(string),
                typeof(SearchTextBox),
                new FrameworkPropertyMetadata(string.Empty));

        public string Placeholder
        {
            get => (string)GetValue(PlaceholderProperty);
            set => SetValue(PlaceholderProperty, value);
        }

        public static readonly DependencyProperty PlaceholderForegroundProperty =
            DependencyProperty.Register(
                nameof(PlaceholderForeground),
                typeof(Brush),
                typeof(SearchTextBox),
                new FrameworkPropertyMetadata(null));

        public Brush? PlaceholderForeground
        {
            get => (Brush?)GetValue(PlaceholderForegroundProperty);
            set => SetValue(PlaceholderForegroundProperty, value);
        }
    }
}
