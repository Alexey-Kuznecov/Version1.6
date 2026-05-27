
namespace UnityCommander.Services.Interfaces
{
    public interface ISelectionService
    {
        void Register(string panelId, ISelectionManager manager);
        void Unregister(string panelId);
        ISelectionManager Get(string panelId);
        ISelectionManager GetActive();
    }
}
