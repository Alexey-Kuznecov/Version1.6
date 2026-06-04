
using Prism.Ioc;
using Prism.Modularity;
using UnityCommander.Modules.FilePanel;
using UnityCommander.Modules.LeftSideBars;
using UnityCommander.Modules.ToolBar;
using UnityCommander.Services.Bootstrap;

namespace UnityCommander
{

    [ModuleDependency(nameof(FilePanelModule))]
    [ModuleDependency(nameof(LeftSideBarsModule))]
    [ModuleDependency(nameof(ToolBarModule))]
    internal class AppLoadModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var initializer = containerProvider.Resolve<AppInitializer>();
            initializer.Initialize();
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
        }
    }
}