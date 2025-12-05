using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace UnityCommander.Modules.FilePanel.Columns
{
    public class ColumnRegistry
    {
        private readonly List<IColumnProvider> providers = new();
        public void RegisterProvider(IColumnProvider provider) => providers.Add(provider);
        public IEnumerable<ColumnModel> GetColumns(PanelType panelType)
           => providers.SelectMany(p => p.GetColumnDefinitions(panelType))
                       .Where(c =>
                           (panelType == PanelType.Files && c.ForFiles) ||
                           (panelType == PanelType.Folders && c.ForFolders) ||
                           (panelType == PanelType.Drives && c.ForDrives))
                       .OrderBy(c => c.Order);
    }
}