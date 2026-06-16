
using System;
using System.Collections.Generic;
using System.Linq;
using UnityCommander.Abstractions.Sidebar;

namespace UnityCommander.Common.Sidebar
{
    public sealed class SidebarRegistry : ISidebarRegistry
    {
        private readonly List<ISidebarSection> _sections = new();

        public event Action<string>? OwnerUnload;

        public void Register(ISidebarSection section)
            => _sections.Add(section);

        public ISidebarSection? Get(string id)
            => _sections.FirstOrDefault(x => x.Id == id);

        public IEnumerable<ISidebarSection> GetAll()
            => _sections;

        public void Unregister(string id)
        {
            var section = Get(id);

            if (section != null)
                _sections.Remove(section);
        }

        public void Cleanup(string ownerId)
        {
            _sections.RemoveAll(x => x.OwnerId == ownerId);
            
            OwnerUnload?.Invoke(ownerId);
        }
    }
}
