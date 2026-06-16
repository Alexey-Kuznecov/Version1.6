
namespace UnityCommander.Abstractions.Columns
{
    public interface IColumnRegistry : IOwnedRegistry
    {
        event Action<string>? PluginUnloaded;

        void RegisterSystemProvider(IColumnProvider provider);

        void RegisterPluginProvider(string pluginId, IColumnProvider provider);

        IEnumerable<ColumnModel> GetColumns(PanelType panelType);

        void Unregister(IColumnProvider provider);
    }
}