
using System.Windows.Controls;

namespace UnityCommander.WPF
{
    public sealed class CursorTarget
    {
        public required ListViewItem Element { get; init; }

        public object? DataContext =>
            Element.DataContext;
    }
}
