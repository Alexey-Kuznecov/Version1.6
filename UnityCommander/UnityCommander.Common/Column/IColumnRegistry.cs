
using System;
using System.Collections.Generic;
using UnityCommander.Common.Columns;

namespace UnityCommander.Modules.FilePanel.Columns
{
    public interface IColumnRegistry
    {
        event Action<string>? PluginUnloaded;

        void RegisterSystemProvider(IColumnProvider provider);

        void RegisterPluginProvider(string pluginId, IColumnProvider provider);

        IEnumerable<ColumnModel> GetColumns(PanelType panelType);

        void Unregister(IColumnProvider provider);

        void Cleanup(string pluginId);
    }
}