
using UnityCommander.Abstractions.Columns;
using UnityCommander.Abstractions.Command;
using UnityCommander.Abstractions.Dialog;
using UnityCommander.Abstractions.Overrides;
using UnityCommander.Abstractions.Plugins;
using UnityCommander.Abstractions.Sidebar;

namespace UnityCommander.Abstractions
{
    public interface IRuntimeServices
    {
        IPluginCommandRegistry Commands { get; }

        ISidebarRegistry Sidebar { get; }

        IDialogRegistry Dialog { get; }

        IColumnRegistry Columns { get; }

        IServiceOverrideRegistry Overrides { get; }

        ICompositionRegistry Composition { get; }

        //IConsoleCommandRegistry Console { get; }

        void Cleanup(string id);
    }
}
