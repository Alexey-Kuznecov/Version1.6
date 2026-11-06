
using System.Windows.Controls;
using UnityCommander.Services.Interfaces.Docking;

namespace UnityCommander.Services.Interfaces
{
    public interface IToolDescriptor
    {
        string Id { get; }

        string Title { get; }

        string Icon { get; }

        string Description { get; }

        string Category { get; }

        bool CanCreateMultiple { get; }

        int MaxInstances { get; }

        ToolDockSide DockSide { get; }

        Control Create();
    }
}
