
using System;

namespace UnityCommander.Modules.LeftSideBars
{
    using Prism.Ioc;
    using Prism.Modularity;
    using Prism.Navigation.Regions;
    using UnityCommander.Core;
    using UnityCommander.Modules.LeftSideBars.ViewModels;
    using UnityCommander.Modules.LeftSideBars.Views;
    using UnityCommander.Services.Interfaces;

    public class LeftSideBarsModule : IModule
    {
        private readonly IRegionManager regionManager;

        public LeftSideBarsModule(IRegionManager regionManager)
        {
            this.regionManager = regionManager;
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            this.regionManager.RequestNavigate(RegionNames.LeftSideBarRegion, nameof(Sidebar));
            
            var coordinator =
                containerProvider.Resolve<ISessionAggregator>();
            var vm =
                containerProvider.Resolve<SidebarViewModel>();

            coordinator.RegisterCapture(vm.Capture);
            coordinator.RegisterRestore(vm.Restore);
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<Sidebar>();
        }
    }
}