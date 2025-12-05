
using System.Collections.Generic;
using System.Linq;
using UnityCommander.Services.Interfaces.Settings;

namespace UnityCommander.Modules.FilePanel.Columns
{
    // Lightweight implementation using existing ISettingsStore
    public class ColumnStateManager : IColumnStateManager
    {
        private readonly ISettingsStore _settings;

        public ColumnStateManager(ISettingsStore settings)
        {
            _settings = settings;
        }

        public List<ColumnModel> LoadState(string panelId, PanelType panelType, IEnumerable<ColumnModel> defaultDefinitions)
        {
            var defs = defaultDefinitions.Select(d => new ColumnModel
            {
                Id = d.Id,
                Header = d.Header,
                DisplayMemberPath = d.DisplayMemberPath,
                CellTemplateResourceKey = d.CellTemplateResourceKey,
                Width = d.Width,
                IsVisible = d.IsVisible,
                Order = d.Order,
                SyncGroup = d.SyncGroup,
                ForFiles = d.ForFiles,
                ForFolders = d.ForFolders,
                ForDrives = d.ForDrives,
                ColumnValueHandler = d.ColumnValueHandler
            }).ToList();

            // Load order
            var savedOrder = _settings.LoadColumnsOrder(panelId) ?? new List<string>();
            if (savedOrder.Any())
            {
                // reorder by saved order, unknown ids go to the end in the original order
                var map = defs.ToDictionary(x => x.Id);
                var ordered = new List<ColumnModel>();
                foreach (var id in savedOrder)
                    if (map.TryGetValue(id, out var cm)) ordered.Add(cm);
                ordered.AddRange(defs.Where(d => !savedOrder.Contains(d.Id)));
                defs = ordered;
            }

            // Load widths
            foreach (var c in defs)
            {
                var w = _settings.LoadColumnWidth(c.Id);
                if (w.HasValue) c.Width = w.Value;
            }


            return defs;
        }

        public void SaveState(string panelId, IEnumerable<ColumnModel> columns)
        {
            var list = columns.ToList();
            _settings.SaveColumnsOrder(panelId, list.Select(c => c.Id).ToList());
            foreach (var c in list)
            {
                _settings.SaveColumnWidth(c.Id, c.Width);
            }
        }
    }
}
