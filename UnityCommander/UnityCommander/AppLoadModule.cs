
using Prism.Ioc;
using Prism.Modularity;
using System.Threading;
using UnityCommander.Common.Diagnostic;
using UnityCommander.Modules.FilePanel;
using UnityCommander.Modules.LeftSideBars;
using UnityCommander.Modules.ToolBar;
using UnityCommander.Services.Background;
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
            var refreshService = containerProvider.Resolve<IBackgroundService>();
            var token = new CancellationToken();
            _ = refreshService.RunAsync(token);
            initializer.Initialize();
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
          
        }

        private static void RegisterDiagnostics(IContainerProvider containerRegistry)
        {
            var diagnostics = containerRegistry.Resolve<IDiagnosticRegistry>();

            var pan = containerRegistry.Resolve<IPanelRegistry>();
            var tab = containerRegistry.Resolve<ITabRegistry>();

            diagnostics.Register(pan as IDiagnosticSource);
            diagnostics.Register(tab as IDiagnosticSource);
        }
    }
}