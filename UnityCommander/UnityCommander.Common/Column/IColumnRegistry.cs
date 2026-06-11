
using System.Collections.Generic;
using UnityCommander.Common.Columns;

namespace UnityCommander.Modules.FilePanel.Columns
{
    public interface IColumnRegistry
    {
        public void RegisterProvider(IColumnProvider provider);

        public IEnumerable<ColumnModel> GetColumns(PanelType panelType);
    }
}