
using Prism.Mvvm;
using System.Collections.Generic;

namespace UnityCommander.Controls.Layout
{
    public abstract class LayoutNode : BindableBase, ILayoutNode
    {
        public LayoutNodeKind Kind { get; set; }

        public SizeSpec Size { get; set; }

        public IReadOnlyList<ILayoutNode> Children { get; set; }
    }
}
