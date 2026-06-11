using System.Collections.Generic;
using UnityCommander.Common.Sidebar;

namespace UnityCommander.Services.Interfaces.Sidebar
{
    public interface ISidebarService
    {
        public void Register(ISidebarDefinition def);
        public void Register(ISidebarSection section);
        public ISidebarSection? Get(string id);
        public IEnumerable<ISidebarSection>? GetAll();
    }
}