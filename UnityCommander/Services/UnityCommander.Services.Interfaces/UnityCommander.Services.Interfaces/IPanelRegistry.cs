
using System;
using System.Collections.Generic;
using UnityCommander.Common;

namespace UnityCommander.Services.Interfaces
{
    public interface IPanelRegistry
    {
        public Guid? ActivePanelId { get; }
        // Панели
        void RegisterPanel(Guid panelId);

        void UnregisterPanel(Guid panelId);

        IPanel GetPanel(Guid panelId);

        IReadOnlyList<IPanel> GetAllPanels();


        // Активная панель
        IPanel GetActivePanel();

        void SetActivePanel(Guid panelId);

        // Работа с вкладками
        void AddTab(Guid panelId, Guid tabId);

        void RemoveTab(Guid tabId);

        void SetActiveTab(Guid panelId, Guid tabId);

        void EnsurePanel(Guid panelId);
    }
}
