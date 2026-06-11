

using System;

namespace UnityCommander.Common.Sidebar
{
    public interface ISidebarSection
    {
        string Id { get; }

        string IconKey { get; }

        ISidebarDefinition Definition { get; }

        Type ViewType { get; }

        Type ViewModel { get; }

        void Activate();

        void Deactivate();

        void Capture(ISidebarSectionState state);

        void Restore(ISidebarSectionState state);
    }
}