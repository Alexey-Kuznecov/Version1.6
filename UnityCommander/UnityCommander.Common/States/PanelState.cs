
using System;
using System.Collections.Generic;
using UnityCommander.Common.Models.Directory;

namespace UnityCommander.Common.State
{
    public class PanelState
    {
        public Guid PanelId { get; set; }   // 💥 ключевая штука
        public PanelMode Mode { get; set; }
        public ViewMode ViewMode { get; set; }
        public List<TabState> Tabs { get; set; } = new();
        public Guid? ActiveTabId { get; set; }
    }
}
