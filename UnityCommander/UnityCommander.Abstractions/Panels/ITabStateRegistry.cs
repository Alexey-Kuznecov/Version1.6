
namespace UnityCommander.Abstractions.Panels
{
    public interface ITabStateRegistry
    {
        void Register(TabState state);
        void Unregister(Guid tabId);
        TabState? Get(Guid tabId);

        ViewMode GetPanelViewMode(Guid panelId);
        void SetPanelViewMode(Guid panelId, ViewMode mode);
    }
}
