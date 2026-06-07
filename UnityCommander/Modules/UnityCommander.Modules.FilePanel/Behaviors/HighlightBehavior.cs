using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace UnityCommander.Modules.FilePanel.Behaviors
{
    public static class HighlightBehavior
    {
        public static readonly DependencyProperty IsHighlightedProperty =
            DependencyProperty.RegisterAttached(
                "IsHighlighted",
                typeof(bool),
                typeof(HighlightBehavior),
                new PropertyMetadata(false, OnHighlightChanged));

        public static void SetIsHighlighted(DependencyObject element, bool value)
            => element.SetValue(IsHighlightedProperty, value);

        public static bool GetIsHighlighted(DependencyObject element)
            => (bool)element.GetValue(IsHighlightedProperty);

        private static void OnHighlightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ListViewItem item)
            {
                item.Background = (bool)e.NewValue
                    ? (Brush)Application.Current.Resources["PrimaryHueLightBrush"]
                    : Brushes.Transparent;
            }
        }
    }
}
