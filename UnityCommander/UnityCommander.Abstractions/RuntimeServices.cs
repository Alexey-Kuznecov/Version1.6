
using UnityCommander.Abstractions.Columns;
using UnityCommander.Abstractions.Command;
using UnityCommander.Abstractions.Dialog;
using UnityCommander.Abstractions.Overrides;
using UnityCommander.Abstractions.Plugins;
using UnityCommander.Abstractions.Sidebar;

namespace UnityCommander.Abstractions
{
    public class RuntimeServices : IRuntimeServices
    {
        public ISidebarRegistry Sidebar { get; }

        public IDialogRegistry Dialog { get; }

        public IColumnRegistry Columns { get; }

        public IServiceOverrideRegistry Overrides { get; }

        public ICompositionRegistry Composition { get; }

        public IPluginCommandRegistry Commands { get; }

        public RuntimeServices(
            ISidebarRegistry sidebar, 
            IDialogRegistry dialog,
            IColumnRegistry registry,
            IServiceOverrideRegistry overrideRegistry,
            ICompositionRegistry compositionRegistry,
            IPluginCommandRegistry command)
        {
            Sidebar = sidebar;
            Dialog = dialog;
            Columns = registry;
            Overrides = overrideRegistry;
            Composition = compositionRegistry;
            Commands = command;
        }

        public void Cleanup(string id)
        {
            Dialog.Cleanup(id);
            Columns.Cleanup(id);
            Sidebar.Cleanup(id);
            Overrides.Cleanup(id);
        }
    }
}
