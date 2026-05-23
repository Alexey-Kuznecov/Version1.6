
using System.Windows.Controls;

namespace UnityCommander.Modules.FilePanel.Layout
{
    public class SplitNode : LayoutNode
    {
        public Orientation Orientation { get; set; } // Horizontal / Vertical

        public double Ratio { get; set; } = 0.5; // 0..1

        public LayoutNode First { get; set; }
        public LayoutNode Second { get; set; }
    }
}