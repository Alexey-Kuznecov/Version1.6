
using Prism.Ioc;
using Prism.Modularity;
using UnityCommander.Common.Diagnostic;
using UnityCommander.Modules.FilePanel;
using UnityCommander.Modules.LeftSideBars;
using UnityCommander.Modules.ToolBar;
using UnityCommander.Services.Bootstrap;
using UnityCommander.Services.Interfaces;

namespace UnityCommander
{

    [ModuleDependency(nameof(FilePanelModule))]
    [ModuleDependency(nameof(LeftSideBarsModule))]
    [ModuleDependency(nameof(ToolBarModule))]
    internal class AppLoadModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            RegisterDiagnostics(containerProvider);
            var initializer = containerProvider.Resolve<AppInitializer>();
            initializer.Initialize();
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
          
        }

        private static void RegisterDiagnostics(IContainerProvider containerRegistry)
        {
            var diagnostics = containerRegistry.Resolve<IDiagnosticRegistry>();

            var pan = containerRegistry.Resolve<IPanelRegistry>();

            diagnostics.Register(pan as IDiagnosticSource);
        }
    }
}