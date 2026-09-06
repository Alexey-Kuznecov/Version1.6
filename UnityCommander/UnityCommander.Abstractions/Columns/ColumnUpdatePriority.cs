
namespace UnityCommander.Abstractions.Columns
{
    public enum ColumnUpdatePriority
    {
        Realtime = 100,
        Normal = 500,
        Background = 1000,
        Ignore,
    }
}
