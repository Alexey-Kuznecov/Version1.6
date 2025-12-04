using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.Services
{
    public class ColumnRegistry
    {
        //private readonly List<IColumnProvider> providers = new();
        //public void RegisterProvider(IColumnProvider provider) => providers.Add(provider);
        //public IEnumerable<ColumnDefinition> GetColumns(PanelType panelType)
        //    => providers.SelectMany(p => p.GetColumnDefinitions(panelType))
        //                .Where(c => (panelType == PanelType.Files && c.ForFiles) || (panelType == PanelType.Folders && c.ForFolders))
        //                .OrderBy(c => c.Order);
    }
}
