using System;
using System.Collections.Generic;

namespace UnityCommander.Common.Panels
{
    public interface IPanel
    {
        Guid PanelId { get; }

        IReadOnlyList<Guid> Tabs { get; }

        Guid? ActiveTabId { get; }

        void AddTab(Guid tabId);
        void RemoveTab(Guid tabId);
        void SetActiveTab(Guid tabId);
    }
}
