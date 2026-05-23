
using System;
using System.Collections.Generic;
using UnityCommander.Common.Docking;
using UnityCommander.Modules.FilePanel.Docking.Snapshot;

namespace UnityCommander.Modules.FilePanel.Docking.Diff
{
    public class DockingDiffEngine : IDockingDiffEngine
    {
        public DiffResult Diff(DockingSnapshot oldSnap, DockingSnapshot newSnap)
        {
            var result = new DiffResult();

            var oldMap = BuildMap(oldSnap);
            var newMap = BuildMap(newSnap);

            // 🔥 Added + Moved
            foreach (var (tabId, newPanel) in newMap)
            {
                if (!oldMap.TryGetValue(tabId, out var oldPanel))
                {
                    result.Operations.Add(new TabOperation()
                    {
                        Type = TabOperationType.Add,
                        TabId = tabId,
                        ToPanelId = newPanel
                    });

                    continue;
                }

                if (oldPanel != newPanel)
                {
                    result.Operations.Add(new TabOperation()
                    {
                        Type = TabOperationType.Move,
                        TabId = tabId,
                        FromPanelId = oldPanel,
                        ToPanelId = newPanel
                    });
                }
            }

            // 🔥 Removed
            foreach (var tabId in oldMap.Keys)
            {
                if (!newMap.ContainsKey(tabId))
                    result.Operations.Add(new TabOperation()
                    {
                        Type = TabOperationType.Remove,
                        TabId = tabId
                    });
            }

            return result;
        }

        private Dictionary<Guid, Guid> BuildMap(DockingSnapshot snap)
        {
            var map = new Dictionary<Guid, Guid>();

            foreach (var panel in snap.Panels)
            {
                foreach (var tab in panel.Tabs)
                {
                    map[tab] = panel.PanelId;
                }
            }

            return map;
        }
    }
}
