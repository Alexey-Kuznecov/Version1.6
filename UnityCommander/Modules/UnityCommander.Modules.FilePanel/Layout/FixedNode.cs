
namespace UnityCommander.Modules.FilePanel.Layout
{
    public class FixedNode : LayoutNode
    {
        public double Size { get; set; } // например 25
        public LayoutNode Content { get; set; }
    }
}
