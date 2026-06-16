
using System;
using System.Collections.Generic;
using UnityCommander.Abstractions.Sidebar;

namespace UnityCommander.Services.Interfaces.Sidebar
{
    public sealed class SidebarService : ISidebarService
    {
        private readonly ISidebarRegistry _registry;

        private readonly ISidebarSectionFactory _factory;

        public event Action<string>? OnCleanup;

        public event Action? Changed;

        public SidebarService(
            ISidebarRegistry registry,
            ISidebarSectionFactory factory)
        {
            _registry = registry;
            _factory = factory;

            registry.OwnerUnload += OwnerUnload;
        }

        private void OwnerUnload(string ownerId)
        {
            Changed?.Invoke();

            OnCleanup?.Invoke(ownerId);
        }

        public void Register(ISidebarSection section)
           => _registry.Register(section);

        public void Register(ISidebarDefinition definition)
        {
            var section = _factory.Create(definition);

            _registry.Register(section);

            Changed?.Invoke();
        }

        public IEnumerable<ISidebarSection>? GetAll()
            => _registry.GetAll();
    }
}
