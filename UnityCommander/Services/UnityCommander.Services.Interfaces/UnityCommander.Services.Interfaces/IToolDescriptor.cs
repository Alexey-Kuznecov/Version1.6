

using System.Windows.Controls;
using UnityCommander.Services.Interfaces.Docking;

namespace UnityCommander.Services.Interfaces
{
    public interface IToolDescriptor
    {
        string Id { get; }

        string Title { get; }

        bool CanCreateMultiple { get; }

        ToolDockSide DockSide { get; }

        Control Create();
    }
}
