

using System;
using UnityCommander.Common.Plugins;

namespace UnityCommander.Common.Sidebar
{
    public interface ISidebarSection : IPluginOwned
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