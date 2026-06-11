
using System;

namespace UnityCommander.Common.Debugger
{
    public class DebugPanelState
    {
        public Guid ActivePanel { get; set; }
        public int PanelCount { get; set; }
        public int TabCount { get; set; }
        public int PrePanelCount { get; set; }
        public int PreTabCount { get; set; }
    }
}
