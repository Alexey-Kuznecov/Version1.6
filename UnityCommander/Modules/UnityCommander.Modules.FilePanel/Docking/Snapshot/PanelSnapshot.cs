
using System;
using System.Collections.Generic;

namespace UnityCommander.Modules.FilePanel.Docking.Snapshot
{
    public class PanelSnapshot
    {
        public Guid PanelId { get; set; }
        public List<Guid> Tabs { get; set; } = new();
        public bool IsFloating { get; set; }
    }
}
