using UnityCommander.Core;
using UnityCommander.Modules.FilePanel.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace UnityCommander.Modules.FilePanel
{
    public class FilePanelModule : IModule
    {
        private readonly IRegionManager _regionManager;

        public FilePanelModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            _regionManager.RequestNavigate(RegionNames.FilePanelRegion, "ViewA");
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<ViewA>();
        }
    }
}