
namespace UnityCommander.Services.Interfaces
{
    public interface ITabContextAccessor
    {
        ITabContentAdapter ActiveTab { get; }
        string ActiveTabId { get; }
        string CurrentPath { get; }
        //ISelectionManager ActiveSelectionManager { get; }
    }
}
