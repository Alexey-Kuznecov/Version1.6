
using System.Collections.Generic;
using System.Windows;

namespace UnityCommander.Controls.Layout
{
    public class LayoutResult
    {
        public ILayoutNode Node { get; init; }
        public Rect Bounds { get; init; }
        public List<LayoutResult> Children { get; init; } = new();
    }
}
