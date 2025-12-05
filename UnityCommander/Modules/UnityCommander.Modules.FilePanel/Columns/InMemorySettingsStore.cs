using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityCommander.Modules.FilePanel.Columns
{
    public class InMemorySettingsStore : ISettingsStore
    {
        private readonly Dictionary<string, double> columnWidths = new();
        private readonly Dictionary<string, List<string>> columnOrders = new();

        public void SaveColumnWidth(string columnId, double width)
        {
            columnWidths[columnId] = width;
        }

        public double? LoadColumnWidth(string columnId)
        {
            return columnWidths.TryGetValue(columnId, out var width) ? width : null;
        }

        public void SaveColumnsOrder(string panelId, List<string> columnIds)
        {
            columnOrders[panelId] = new List<string>(columnIds);
        }

        public List<string> LoadColumnsOrder(string panelId)
        {
            return columnOrders.TryGetValue(panelId, out var order) ? new List<string>(order) : new List<string>();
        }
    }
}
