
namespace UnityCommander.Abstractions.Columns
{
    public interface IColumnProvider
    {
        IEnumerable<ColumnModel> GetColumnDefinitions(PanelType panelType);
    }
}
