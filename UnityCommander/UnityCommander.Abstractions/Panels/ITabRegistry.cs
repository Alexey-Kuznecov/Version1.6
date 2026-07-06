
namespace UnityCommander.Abstractions.Panels
{
    public interface ITabRegistry
    {
        ITabContentAdapter ActiveTab { get; }

        IReadOnlyList<ITabContentAdapter> GetAllTabs();

        ITabContentAdapter GetTab(Guid tabId);

        void Register(ITabContentAdapter tab);

        bool Contains(Guid tabId);

        void Unregister(Guid tabId);

        void SetActive(Guid tabId);
    }
}
