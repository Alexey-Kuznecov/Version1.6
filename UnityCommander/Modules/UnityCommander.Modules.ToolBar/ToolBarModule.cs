
namespace UnityCommander.Modules.ToolBar
{
    using Prism.Ioc;
    using Prism.Modularity;
    using Prism.Regions;

    using UnityCommander.Core;
    using UnityCommander.Modules.ToolBar.ViewModels;
    using UnityCommander.Modules.ToolBar.Views;
    using UnityCommander.Services.Interfaces;

    public class ToolBarModule : IModule
    {
        private readonly IRegionManager regionManager;

        public ToolBarModule(IRegionManager regionManager)
        {
            this.regionManager = regionManager;
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            this.regionManager.RequestNavigate(RegionNames.ToolBarRegion, "ToolBarView");

            var coordinator = 
                containerProvider.Resolve<ISessionAggregator>();
            var vm =
                containerProvider.Resolve<ToolBarViewModel>();

            coordinator.RegisterCapture(vm.Capture);
            coordinator.RegisterRestore(vm.Restore);
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<ToolBarView>();
        }
    }
}