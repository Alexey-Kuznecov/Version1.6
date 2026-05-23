
using System.Collections.Generic;
using System.Windows.Controls;

namespace UnityCommander.Modules.FilePanel.Layout
{
    public class StackNode : LayoutNode
    {
        public Orientation Orientation { get; set; }

        public List<LayoutNode> Children { get; set; } = new();
    }
}
