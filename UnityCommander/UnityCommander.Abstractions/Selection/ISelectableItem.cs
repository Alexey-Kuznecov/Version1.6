
namespace UnityCommander.Abstractions.Selection
{
    public interface ISelectableItem
    {
        bool IsSelected { get; set; }
        string Key { get; }
    }
}
