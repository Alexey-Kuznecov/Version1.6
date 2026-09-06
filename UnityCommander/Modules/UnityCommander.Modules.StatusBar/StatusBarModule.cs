
using UnityCommander.Core;
using UnityCommander.Modules.StatusBar.Views;

namespace UnityCommander.Modules.StatusBar
{
    public class StatusBarModule : IModule
    {
        private readonly IRegionManager regionManager;

        public StatusBarModule(IRegionManager regionManager)
        {
            this.regionManager = regionManager;
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            regionManager.RequestNavigate(RegionNames.StatusBarRegion, nameof(StatusBarView));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<StatusBarView>();
        }
    }
}
