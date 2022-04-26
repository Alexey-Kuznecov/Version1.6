using System.Windows;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using UnityCommander.Core;
using UnityCommander.Modules.TabPanel.Views;

namespace UnityCommander.Modules.TabPanel
{
    public class TabPanelModule : IModule
    {        
        /// <summary>
        /// The region manager.
        /// </summary>
        private readonly IRegionManager regionManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="TabPanelModule"/> class.
        /// </summary>
        /// <param name="regionManager"> The region manager. </param>
        public TabPanelModule(IRegionManager regionManager)
        {
            this.regionManager = regionManager;
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            this.regionManager.RequestNavigate(RegionNames.TabPanelRegion, nameof(TabPanelView));
            this.regionManager.RequestNavigate(NestedRegionNames.LeftFilePanelRegion, nameof(LeftPanelContentView), this.NavigationCallback);
            this.regionManager.RequestNavigate(NestedRegionNames.RightFilePanelRegion, nameof(RightPanelContentView), this.NavigationCallback);
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<TabPanelView>();
            containerRegistry.RegisterForNavigation<LeftPanelContentView>();
            containerRegistry.RegisterForNavigation<RightPanelContentView>();
        }

        /// <summary>
        /// The navigation callback.
        /// </summary>
        /// <param name="result">
        /// The result.
        /// </param>
        private void NavigationCallback(NavigationResult result)
        {
            var region = result.Context.NavigationService.Region;

            foreach (var view in region.Views)
            {
                if (view is FrameworkElement vm)
                {
                    if (vm.DataContext is IPanelContainer container)
                    {
                        container.InitialDirectoryPanel(region.Name);
                    }
                }
            }
        }
    }
}