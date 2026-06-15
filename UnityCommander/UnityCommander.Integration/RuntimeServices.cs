
using UnityCommander.Common.Dialog;
using UnityCommander.Modules.FilePanel.Columns;
using UnityCommander.Services.Interfaces.Sidebar;

namespace UnityCommander.Integration
{
    public class RuntimeServices : IRuntimeServices
    {
        public ISidebarService Sidebar { get; }

        public IDialogRegistry Dialog { get; }

        public IColumnRegistry Columns { get; }

        public RuntimeServices(
            ISidebarService sidebar, 
            IDialogRegistry dialog,
            IColumnRegistry registry)
        {
            Sidebar = sidebar;
            Dialog = dialog;
            Columns = registry;
        }

        public void Cleanup(string id)
        {
            Dialog.Cleanup(id);
            Columns.Cleanup(id);
            Sidebar.Cleanup(id);
        }
    }
}
