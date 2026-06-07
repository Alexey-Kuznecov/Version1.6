

using System.Windows.Controls;

namespace UnityCommander.Common.Sidebar
{
    public interface ISidebarSection
    {
        string Id { get; }

        string IconKey { get; }

        ISidebarDefinition Definition { get; }

        UserControl View { get; }

        object? ViewModel { get; }

        void Activate();

        void Deactivate();

        void Capture(ISidebarSectionState state);

        void Restore(ISidebarSectionState state);
    }
}