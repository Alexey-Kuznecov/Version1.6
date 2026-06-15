
using UnityCommander.Common.Columns;

namespace UnityCommander.Common.Column
{
    public sealed class ColumnProviderEntry
    {
        public string PluginId { get; }
        public IColumnProvider Provider { get; }

        public ColumnProviderEntry(string pluginId, IColumnProvider provider)
        {
            PluginId = pluginId;
            Provider = provider;
        }
    }
}
