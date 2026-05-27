
using UnityCommander.Common.Models;
using UnityCommander.Modules.FilePanel.Models;

namespace UnityCommander.Modules.FilePanel.Layout
{
    public class ContentNode : LayoutNode
    {
        public string Key { get; set; }
        public ContentRole Role { get; internal set; }
        public ViewMode? ViewMode { get; internal set; }
        public object Context { get; set; }
    }
}
