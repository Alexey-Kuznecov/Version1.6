using System;
using System.Collections.Generic;
using UnityCommander.Common.Sidebar;

namespace UnityCommander.Services.Interfaces.Sidebar
{
    public interface ISidebarService
    {
        public event Action<string>? PluginUnloaded;

        public void Register(ISidebarDefinition def);
        public void Register(ISidebarSection section);
        public ISidebarSection? Get(string id);
        public IEnumerable<ISidebarSection>? GetAll();
        void Unregister(string id);

        void Cleanup(string pluginId);
    }
}