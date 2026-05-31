
using System.Collections.Generic;

namespace UnityCommander.Controls.Layout
{
    public interface ILayoutNode
    {
        LayoutNodeKind Kind { get; }
        SizeSpec Size { get; }
        IReadOnlyList<ILayoutNode> Children { get; }
    }
}
