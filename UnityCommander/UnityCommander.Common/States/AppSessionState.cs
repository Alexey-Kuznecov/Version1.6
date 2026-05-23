

using System.Collections.Generic;

namespace UnityCommander.Common.State
{
    public class AppSessionState
    {
        public List<PanelState> Panels { get; set; } = new();

        public bool IsConsoleOpen { get; set; }
        public bool IsRibbonOpen { get; set; }
        public bool IsSidebarOpen { get; set; }

        public LayoutState Layout { get; set; }
    }
}
