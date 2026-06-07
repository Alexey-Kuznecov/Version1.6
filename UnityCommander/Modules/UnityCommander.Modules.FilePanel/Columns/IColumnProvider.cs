
using System.Collections.Generic;

namespace UnityCommander.Modules.FilePanel.Columns
{
    public interface IColumnProvider
    {
        IEnumerable<ColumnModel> GetColumnDefinitions(PanelType panelType);
    }
}
