
using System;
using System.Collections.Generic;
using System.Linq;
using UnityCommander.Common.Columns;

namespace UnityCommander.Modules.FilePanel.Columns
{
    public class ColumnRegistry : IColumnRegistry
    {
        private readonly List<IColumnProvider> providers = new();
        
        public ColumnRegistry(IEnumerable<IColumnProvider> providers)
        {
            this.providers = providers.ToList();
        }

        public void RegisterProvider(IColumnProvider provider)
        {
            if (!providers.Contains(provider))
                providers.Add(provider);
        }

        public IEnumerable<ColumnModel> GetColumns(PanelType panelType)
        {
            var columns = providers
                .SelectMany(p => p.GetColumnDefinitions(panelType))
                .ToList();

            var duplicates = columns
                .GroupBy(c => c.Id)
                .Where(g => g.Count() > 1)
                .ToList();

            if (duplicates.Any())
            {
                throw new InvalidOperationException(
                    $"Duplicate column ids: {string.Join(", ", duplicates.Select(x => x.Key))}");
            }

            return columns.OrderBy(c => c.Order);
        }

        internal object GetColumns(object panelType)
        {
            throw new NotImplementedException();
        }
    }
}