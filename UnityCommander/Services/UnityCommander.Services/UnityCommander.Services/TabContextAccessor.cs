
using System;

namespace UnityCommander.Services.Interfaces
{
    public class TabContextAccessor : ITabContextAccessor
    {
        private readonly ITabRegistry _registry;

        public ITabContentAdapter ActiveTab => _registry.ActiveTab;

        public Guid ActiveTabId => _registry.ActiveTab.TabId;
        
        public string CurrentPath => _registry.ActiveTab.GetCurrentPath();
       
        public TabContextAccessor(ITabRegistry tabRegistry)
        {
            _registry = tabRegistry;
        }
    }
}
