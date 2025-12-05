using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityCommander.Modules.FilePanel.Columns
{
    public interface ISettingsStore
    {
        void SaveColumnWidth(string columnId, double width);
        double? LoadColumnWidth(string columnId);
        void SaveColumnsOrder(string panelId, List<string> columnIds);
        List<string> LoadColumnsOrder(string panelId);
    }
}
