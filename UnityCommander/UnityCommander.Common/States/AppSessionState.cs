

using System.Collections.Generic;
using UnityCommander.Common.States;

namespace UnityCommander.Common.State
{
    public class AppSessionState
    {
        public List<PanelSessionState> Panels { get; set; } = new();

        public bool IsConsoleOpen { get; set; }

        public SidebarSessionState Sidebar { get; set; }

        public BottomPanelSessionState BottomPanel { get; set; }

        public RibbonSessionState Ribbon { get; set; }
    }
}
