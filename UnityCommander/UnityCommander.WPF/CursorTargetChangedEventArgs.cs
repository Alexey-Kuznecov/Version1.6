
using System.Windows;
using System.Windows.Controls;

namespace UnityCommander.WPF
{
    public sealed class CursorTargetChangedEventArgs : RoutedEventArgs
    {
        public ListViewItem? Previous { get; }
        public ListViewItem? Current { get; }

        public CursorTargetChangedEventArgs(
            ListViewItem? previous,
            ListViewItem? current)
        {
            Previous = previous;
            Current = current;
        }
    }
}
