
using System;
using System.Collections.Generic;
using UnityCommander.Abstractions.Sidebar;

namespace UnityCommander.Services.Interfaces.Sidebar
{
    public interface ISidebarService
    {
        event Action<string> OnCleanup;

        event Action Changed;

        void Register(ISidebarSection section);

        void Register(ISidebarDefinition definition);

        public IEnumerable<ISidebarSection>? GetAll();
    }
}