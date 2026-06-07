
using System.Collections.Generic;
using System.Linq;
using UnityCommander.Common.Sidebar;

namespace UnityCommander.Services.Interfaces.Sidebar
{
    public class SidebarService
    {
        private readonly ISidebarSectionFactory _factory;

        private readonly List<ISidebarSection> _sections = new();

        public IReadOnlyList<ISidebarSection> Sections => _sections;

        public SidebarService(ISidebarSectionFactory factory)
        {
            _factory = factory;
        }

        public void Register(ISidebarDefinition def)
        {
            var section = _factory.Create(def);
            _sections.Add(section);
        }

        public void Register(ISidebarSection section)
        {
            _sections.Add(section);
        }

        public ISidebarSection? Get(string id)
            => _sections.FirstOrDefault(x => x.Id == id);

        public IEnumerable<ISidebarSection>? GetAll()
            => _sections;
    }
}
