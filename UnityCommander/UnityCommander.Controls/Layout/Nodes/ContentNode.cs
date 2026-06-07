
using System;
using UnityCommander.Common.Models;

namespace UnityCommander.Controls.Layout
{
    public class ContentNode : LayoutNode
    {
        public Guid RuntimeId { get; }
        public ContentRole Role { get; set; }
        public ViewMode? ViewMode { get; set; }
        public object Context { get; set; }
    }
}
