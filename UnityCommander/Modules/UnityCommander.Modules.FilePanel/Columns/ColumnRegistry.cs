
using System;
using System.Collections.Generic;
using System.Linq;
using UnityCommander.Abstractions.Columns;

namespace UnityCommander.Modules.FilePanel.Columns
{
    public class ColumnRegistry : IColumnRegistry
    {
        private readonly List<ColumnProviderEntry> _entries = new();

        private Dictionary<PanelType, List<ColumnModel>> _cache = new();
        private Dictionary<PanelType, int> _version = new();

        public event Action<string>? PluginUnloaded;

        public ColumnRegistry(IEnumerable<IColumnProvider> systemProviders)
        {
            foreach (var provider in systemProviders)
            {
                RegisterSystemProvider(provider);
            }
        }

        public void RegisterSystemProvider(IColumnProvider provider)
        {
            _entries.Add(new ColumnProviderEntry(null, provider));
        }

        public void RegisterPluginProvider(string pluginId, IColumnProvider provider)
        {
            _entries.Add(new ColumnProviderEntry(pluginId, provider));
        }

        public IEnumerable<ColumnModel> GetColumns(PanelType panelType)
        {
            return GetAllColumns(panelType)
                .Where(x => x.IsVisible)
                .OrderBy(x => x.Order);
        }

        public IEnumerable<ColumnModel> GetAllColumns(PanelType panelType)
        {
            var columns = _entries
                .SelectMany(e => e.Provider.GetColumnDefinitions(panelType))
                .ToList();

            var duplicates = columns
                .GroupBy(c => c.Id)
                .Where(g => g.Count() > 1)
                .ToList();

            if (duplicates.Any())
                throw new InvalidOperationException(
                    $"Duplicate column ids: {string.Join(", ", duplicates.Select(x => x.Key))}");

            return columns;
        }

        public void Unregister(IColumnProvider provider)
        {
            _entries.RemoveAll(e => e.Provider == provider);
        }

        public void Cleanup(string pluginId)
        {
            _entries.RemoveAll(e => e.PluginId == pluginId);

            PluginUnloaded?.Invoke(pluginId);
        }
    }
}