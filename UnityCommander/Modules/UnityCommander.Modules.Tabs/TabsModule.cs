using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using UnityCommander.Core;
using UnityCommander.Modules.Tabs.Views;

namespace UnityCommander.Modules.Tabs
{
    public class TabsModule : IModule
    {        
        /// <summary>
        /// The region manager.
        /// </summary>
        private readonly IRegionManager regionManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="TabsModule"/> class.
        /// </summary>
        /// <param name="regionManager"> The region manager. </param>
        public TabsModule(IRegionManager regionManager)
        {
            this.regionManager = regionManager;
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            this.regionManager.RequestNavigate(RegionNames.TabPanelRegion, nameof(TabPanelView));
            //this.regionManager.RequestNavigate("LeftFilePanelRegion.", nameof(LeftPanelContentView));
            //this.regionManager.RequestNavigate("RightFilePanelRegion", nameof(LeftPanelContentView));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<TabPanelView>();
        }
    }
}