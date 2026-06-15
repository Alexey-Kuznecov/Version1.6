
using UnityCommander.Common.Dialog;
using UnityCommander.Modules.FilePanel.Columns;
using UnityCommander.Services.Interfaces.Sidebar;

namespace UnityCommander.Integration
{
    public interface IRuntimeServices
    {
        ISidebarService Sidebar { get; }

        IDialogRegistry Dialog { get; }

        IColumnRegistry Columns { get; }

        //IConsoleCommandRegistry Console { get; }

        void Cleanup(string id);
    }
}
