
using System;
using System.Collections.Generic;
using System.Linq;
using UnityCommander.Common.Sidebar;

namespace UnityCommander.Services.Interfaces.Sidebar
{
    public class SidebarService : ISidebarService
    {
        private readonly ISidebarSectionFactory _factory;

        private readonly List<ISidebarSection> _sections = new();

        public event Action Changed;

        public event Action<string> PluginUnloaded;

        public IReadOnlyList<ISidebarSection> Sections => _sections;

        public SidebarService(ISidebarSectionFactory factory)
        {
            _factory = factory;
        }

        private void NotifyChanged()
        {
            Changed?.Invoke();
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

        public void Unregister(string id)
        {
            throw new System.NotImplementedException();
        }

        public void Cleanup(string pluginId)
        {
            var section = _sections
               .Where(x => x.PluginId == pluginId)
               .Select(x => x)
               .ToList();

            foreach (var id in section)
            {
                _sections.Remove(id);
            }

            PluginUnloaded?.Invoke(pluginId);
        }
    }
}
