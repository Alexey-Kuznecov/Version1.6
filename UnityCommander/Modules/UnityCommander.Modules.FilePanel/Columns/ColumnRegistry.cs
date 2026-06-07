
using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityCommander.Modules.FilePanel.Columns
{
    public class ColumnRegistry
    {
        private readonly List<IColumnProvider> providers = new();
        public ColumnRegistry(IEnumerable<IColumnProvider> providers)
        {
            this.providers = providers.ToList();
        }
        public IEnumerable<ColumnModel> GetColumns(PanelType panelType)
           => providers.SelectMany(p => p.GetColumnDefinitions(panelType))
                       .Where(c =>
                           (panelType == PanelType.Files && c.ForFiles) ||
                           (panelType == PanelType.Folders && c.ForFolders) ||
                           (panelType == PanelType.Drives && c.ForDrives))
                       .OrderBy(c => c.Order);

        internal object GetColumns(object panelType)
        {
            throw new NotImplementedException();
        }
    }
}