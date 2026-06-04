
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityCommander.Services.Settings;

namespace UnityCommander.Modules.FilePanel.Columns
{
    public static class ColumnSyncService
    {
        // Sync groups -> list of subscribers (panelId + column id)
        private static readonly ConcurrentDictionary<string, List<Action<double>>> _groupHandlers = new();
        private static readonly IColumnStateManager _stateManager = new ColumnStateManager(new InMemoryColumnSettingsStore());

        public static void RegisterHandler(string syncGroup, Action<double> handler)
        {
            if (string.IsNullOrEmpty(syncGroup)) return;
            var list = _groupHandlers.GetOrAdd(syncGroup, _ => new List<Action<double>>());
            lock (list)
            {
                list.Add(handler);
            }
        }

        public static void UnregisterHandler(string syncGroup, Action<double> handler)
        {
            if (string.IsNullOrEmpty(syncGroup)) return;
            if (_groupHandlers.TryGetValue(syncGroup, out var list))
            {
                lock (list) { list.Remove(handler); }
            }
        }

        // Called by behavior when a user changed a column width
        public static void NotifyWidthChanged(string syncGroup, double newWidth)
        {
            if (string.IsNullOrEmpty(syncGroup)) return;
            if (!_groupHandlers.TryGetValue(syncGroup, out var list)) return;
            // copy to avoid race
            Action<double>[] handlers;
            lock (list) { handlers = list.ToArray(); }
            foreach (var h in handlers)
            {
                try { h(newWidth); } catch { /* swallow handlers' exceptions */ }
            }
        }

        // Save helper
        public static void SavePanelState(string panelId, IEnumerable<ColumnModel> columns)
        {
            _stateManager.SaveState(panelId, columns);
        }
    }
}
