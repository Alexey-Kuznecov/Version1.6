
using System;
using System.Collections.Generic;

namespace UnityCommander.Services.Interfaces
{
    public interface ITabRegistry
    {
        ITabContentAdapter ActiveTab { get; }

        IReadOnlyList<ITabContentAdapter> GetAllTabs();

        ITabContentAdapter GetTab(Guid tabId);

        void Register(ITabContentAdapter tab);
        void Unregister(Guid tabId);

        void SetActive(Guid tabId);
    }
}
