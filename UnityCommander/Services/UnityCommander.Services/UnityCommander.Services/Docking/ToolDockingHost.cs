
using AvalonDock.Layout;
using System.Collections.Generic;
using UnityCommander.Services.Interfaces.Docking;

namespace UnityCommander.Services.Docking
{
    public sealed class ToolDockingHost
    {
        private readonly Dictionary<ToolDockSide, LayoutAnchorablePane> _panes = new();

        public void Register(
            ToolDockSide side,
            LayoutAnchorablePane pane)
        {
            _panes[side] = pane;
        }

        public LayoutAnchorablePane? GetPane(ToolDockSide side)
        {
            _panes.TryGetValue(side, out var pane);
            return pane;
        }
    }
}
