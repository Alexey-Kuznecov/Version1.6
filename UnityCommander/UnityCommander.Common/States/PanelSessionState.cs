
using System;
using System.Collections.Generic;
using UnityCommander.Common.Models;
using UnityCommander.Common.Models.Directory;

namespace UnityCommander.Common.State
{
    public class PanelSessionState
    {
        public Guid PanelId { get; set; }   // 💥 ключевая штука
        public PanelMode Mode { get; set; }
        public ViewMode ViewMode { get; set; }
        public List<TabSessionState> Tabs { get; set; } = new();
        public Guid? ActiveTabId { get; set; }
    }
}
