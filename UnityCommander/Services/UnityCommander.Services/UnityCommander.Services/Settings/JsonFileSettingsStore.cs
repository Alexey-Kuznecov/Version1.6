
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using UnityCommander.Services.Interfaces.Settings;

namespace UnityCommander.Services.Settings
{
    public class JsonFileSettingsStore : ISettingsStore
    {
        private readonly string _path;
        private SettingsFile _cache;

        public JsonFileSettingsStore(string path)
        {
            _path = path;
            LoadFromDisk();
        }

        private void LoadFromDisk()
        {
            if (!File.Exists(_path)) { _cache = new SettingsFile(); return; }
            var json = File.ReadAllText(_path);
            _cache = JsonSerializer.Deserialize<SettingsFile>(json) ?? new SettingsFile();
        }

        private void SaveToDisk() => File.WriteAllText(_path, JsonSerializer.Serialize(_cache));

        public void SaveColumnWidth(string columnId, double width)
        {
            _cache.ColumnWidths[columnId] = width;
            SaveToDisk();
        }

        public double? LoadColumnWidth(string columnId)
        {
            return _cache.ColumnWidths.TryGetValue(columnId, out var w) ? w : null;
        }

        public void SaveColumnsOrder(string panelId, List<string> columnIds)
        {
            _cache.ColumnOrders[panelId] = columnIds;
            SaveToDisk();
        }

        public List<string> LoadColumnsOrder(string panelId)
        {
            return _cache.ColumnOrders.TryGetValue(panelId, out var o) ? o : new List<string>();
        }

        private class SettingsFile
        {
            public Dictionary<string, double> ColumnWidths { get; set; } = new();
            public Dictionary<string, List<string>> ColumnOrders { get; set; } = new();
        }
    }
}
