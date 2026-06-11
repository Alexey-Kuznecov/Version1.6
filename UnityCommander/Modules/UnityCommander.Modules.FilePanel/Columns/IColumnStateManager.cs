
using System.Collections.Generic;
using UnityCommander.Common.Columns;

namespace UnityCommander.Modules.FilePanel.Columns
{
    public interface IColumnStateManager
    {
        List<ColumnModel> LoadState(string panelId, PanelType panelType, IEnumerable<ColumnModel> defaultDefinitions);
        void SaveState(string panelId, IEnumerable<ColumnModel> columns);
    }
}
