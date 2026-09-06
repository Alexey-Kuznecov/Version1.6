
using UnityCommander.Common.Panels;

namespace UnityCommander.Abstractions.Panels
{
    public interface IPanelRegistry
    {
        event Action<TabAddedEvent> TabAdded;

        event Action<TabRemovedEvent> TabRemoved;

        event Action<ActiveTabChangedEvent> ActiveTabChanged;

        public Guid? ActivePanelId { get; }

        IPanel GetPanel(Guid panelId);

        IReadOnlyList<IPanel> GetAllPanels();

        // Активная панель
        IPanel GetActivePanel();

        void SetActivePanel(Guid panelId);

        Guid? FindPanelByTab(Guid tabId);

        // Работа с вкладками
        void AddTab(Guid panelId, Guid tabId);
        
        void MoveTab(Guid panelId, Guid tabId);

        void RemoveTab(Guid tabId);

        void RemovePanel(Guid panelId);
        
        bool SetActiveTab(Guid panelId, Guid tabId);

        bool Contains(Guid tabId);

        void EnsurePanel(Guid panelId);
        
        bool IsEmpty(Guid panelId);
    }
}
