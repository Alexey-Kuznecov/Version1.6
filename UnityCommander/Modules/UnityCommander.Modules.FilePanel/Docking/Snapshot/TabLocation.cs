
using System;

namespace UnityCommander.Modules.FilePanel.Docking.Snapshot
{
    public class TabLocation
    {
        public Guid TabId { get; set; }
        public Guid PanelId { get; set; }

        public TabHostType HostType { get; set; } // Docked / Floating
        public bool IsFloating { get; internal set; }
    }
}
