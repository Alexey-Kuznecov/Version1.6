

using UnityCommander.Abstractions.Columns;
using UnityCommander.Abstractions.Dialog;
using UnityCommander.Abstractions.Overrides;
using UnityCommander.Abstractions.Sidebar;

namespace UnityCommander.Abstractions
{
    public interface IRuntimeServices
    {
        ISidebarRegistry Sidebar { get; }

        IDialogRegistry Dialog { get; }

        IColumnRegistry Columns { get; }

        IServiceOverrideRegistry OverrideRegistry { get; }

        //IConsoleCommandRegistry Console { get; }

        void Cleanup(string id);
    }
}
