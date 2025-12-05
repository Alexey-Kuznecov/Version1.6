
using System.Collections.Generic;
using UnityCommander.Services.Interfaces.Settings;

namespace UnityCommander.Services.Settings
{
    public class InMemorySettingsStore : ISettingsStore
    {
        private readonly Dictionary<string, double> columnWidths = new();
        private readonly Dictionary<string, List<string>> columnOrders = new();
        public void SaveColumnWidth(string columnId, double width) => columnWidths[columnId] = width;
        public double? LoadColumnWidth(string columnId) => columnWidths.TryGetValue(columnId, out var w) ? w : null;
        public void SaveColumnsOrder(string panelId, List<string> columnIds) => columnOrders[panelId] = new List<string>(columnIds);
        public List<string> LoadColumnsOrder(string panelId) => columnOrders.TryGetValue(panelId, out var o) ? new List<string>(o) : new List<string>();
    }
}
