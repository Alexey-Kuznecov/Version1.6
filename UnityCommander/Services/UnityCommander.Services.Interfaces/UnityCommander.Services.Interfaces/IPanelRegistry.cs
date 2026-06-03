
using System;
using System.Collections.Generic;
using UnityCommander.Common.Panels;

namespace UnityCommander.Services.Interfaces
{
    public interface IPanelRegistry
    {
        event Action<TabAddedEvent> TabAdded;

        event Action<TabActionEvent> TabRemoved;

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

        void EnsurePanel(Guid panelId);
        
        bool IsEmpty(Guid panelId);
    }
}
