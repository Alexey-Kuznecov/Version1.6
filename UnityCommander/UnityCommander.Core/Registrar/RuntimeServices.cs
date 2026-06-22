
using UnityCommander.Abstractions;
using UnityCommander.Abstractions.Columns;
using UnityCommander.Abstractions.Command;
using UnityCommander.Abstractions.Dialog;
using UnityCommander.Abstractions.Overrides;
using UnityCommander.Abstractions.Plugins;
using UnityCommander.Abstractions.Ribbon;
using UnityCommander.Abstractions.Sidebar;
using UnityCommander.Rendering.Icons;

namespace UnityCommander.Core.Registrar
{
    public class RuntimeServices : IRuntimeServices
    {
        public ISidebarRegistry Sidebar { get; }

        public IDialogRegistry Dialog { get; }

        public IColumnRegistry Columns { get; }

        public IServiceOverrideRegistry Overrides { get; }

        public ICompositionRegistry Composition { get; }

        public IPluginCommandRegistry Commands { get; }
        
        public IIconSourceRegistry Icons { get; }

        public IRibbonRegistry Ribbon { get; }

        public RuntimeServices(
            ISidebarRegistry sidebar, 
            IDialogRegistry dialog,
            IColumnRegistry registry,
            IServiceOverrideRegistry overrideRegistry,
            ICompositionRegistry compositionRegistry,
            IPluginCommandRegistry command,
            IRibbonRegistry ribbon,
            IIconSourceRegistry iconSource)
        {
            Sidebar = sidebar;
            Dialog = dialog;
            Columns = registry;
            Overrides = overrideRegistry;
            Composition = compositionRegistry;
            Commands = command;
            Ribbon = ribbon;
            Icons = iconSource;
        }

        public void Cleanup(string id)
        {
            Dialog.Cleanup(id);
            Columns.Cleanup(id);
            Sidebar.Cleanup(id);
            Overrides.Cleanup(id);
            Composition.Cleanup(id);
            Commands.Cleanup(id);
            //Ribbon.Cleanup(id);
        }
    }
}
