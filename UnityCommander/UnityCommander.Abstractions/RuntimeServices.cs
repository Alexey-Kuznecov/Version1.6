
using UnityCommander.Abstractions.Columns;
using UnityCommander.Abstractions.Dialog;
using UnityCommander.Abstractions.Overrides;
using UnityCommander.Abstractions.Sidebar;

namespace UnityCommander.Abstractions
{
    public class RuntimeServices : IRuntimeServices
    {
        public ISidebarRegistry Sidebar { get; }

        public IDialogRegistry Dialog { get; }

        public IColumnRegistry Columns { get; }

        public IServiceOverrideRegistry OverrideRegistry { get; }

        public RuntimeServices(
            ISidebarRegistry sidebar, 
            IDialogRegistry dialog,
            IColumnRegistry registry,
            IServiceOverrideRegistry overrideRegistry)
        {
            Sidebar = sidebar;
            Dialog = dialog;
            Columns = registry;
            OverrideRegistry = overrideRegistry;
        }

        public void Cleanup(string id)
        {
            Dialog.Cleanup(id);
            Columns.Cleanup(id);
            Sidebar.Cleanup(id);
            OverrideRegistry.Cleanup(id);
        }
    }
}
