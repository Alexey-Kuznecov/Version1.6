
using System.Collections.Generic;

namespace UnityCommander.Services.Interfaces.Settings
{
    public interface ISettingsStore
    {
        void SaveColumnWidth(string columnId, double width);
        double? LoadColumnWidth(string columnId);
        void SaveColumnsOrder(string panelId, List<string> columnIds);
        List<string> LoadColumnsOrder(string panelId);
    }
}
