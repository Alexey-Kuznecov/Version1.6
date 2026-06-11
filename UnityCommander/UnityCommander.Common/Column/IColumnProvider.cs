
using System.Collections.Generic;

namespace UnityCommander.Common.Columns
{
    public interface IColumnProvider
    {
        IEnumerable<ColumnModel> GetColumnDefinitions(PanelType panelType);
    }
}
