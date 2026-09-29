
using System;
using UnityCommander.Common.Models;

namespace UnityCommander.Controls.Layout
{
    public class ContentNode : LayoutNode
    {
        public Guid RuntimeId { get; }

        public ContentRole Role { get; set; }

        private ViewMode? _viewMode;

        public ViewMode? ViewMode
        {
            get => _viewMode;
            set => SetProperty(ref _viewMode, value);
        }

        public object Context { get; set; }
    }
}
