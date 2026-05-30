
using System;

namespace UnityCommander.Modules.FilePanel.Layout
{
    public class RegionNode : LayoutNode
    {
        private ContentNode? _content;

        public ContentNode? Content
        {
            get => _content;
            set => SetProperty(ref _content, value);
        }
    }
}
