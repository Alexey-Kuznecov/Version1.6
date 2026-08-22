
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace UnityCommander.WPF
{
    public sealed class CursorTargetService
      : ICursorTargetService
    {
        private readonly Dictionary<UIElement, CursorTarget> _targets = new();

        public CursorTarget? GetCurrent(UIElement source)
        {
            return _targets.TryGetValue(
                source,
                out var target)
                    ? target
                    : null;
        }

        public void Update(
            UIElement source,
            Point position)
        {
            if (source is not ListView listView)
                return;

            var item = GetItemUnderCursor(
                listView,
                position);

            if (_targets.TryGetValue(source, out var current) &&
                ReferenceEquals(current.Element, item))
            {
                return;
            }

            if (item is null)
            {
                _targets.Remove(source);
                return;
            }

            _targets[source] = new CursorTarget
            {
                Element = item
            };
        }

        public static ListViewItem? GetItemUnderCursor(
           ListView listView,
           Point position)
        {
            var hit = listView.InputHitTest(position) as DependencyObject;

            while (hit != null)
            {
                if (hit is ListViewItem item)
                    return item;

                hit = VisualTreeHelper.GetParent(hit);
            }

            return null;
        }

        public void Clear(UIElement source)
        {
            _targets.Remove(source);
        }
    }
}
