using UnityCommander.Core;
using UnityCommander.Modules.LeftSideBars.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace UnityCommander.Modules.LeftSideBars
{
    public class LeftSideBarsModule : IModule
    {
        private readonly IRegionManager _regionManager;

        public LeftSideBarsModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            _regionManager.RequestNavigate(RegionNames.LeftSideBarRegion, "ViewA");
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<ViewA>();
        }
    }
}