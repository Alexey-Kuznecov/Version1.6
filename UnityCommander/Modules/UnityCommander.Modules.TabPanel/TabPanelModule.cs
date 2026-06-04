
namespace UnityCommander.Modules.TabPanel
{
    using System.Windows;

    using Prism.Ioc;
    using Prism.Modularity;
    using Prism.Regions;

    using UnityCommander.Core;
    using UnityCommander.Modules.FilePanel.Views;
    using UnityCommander.Modules.TabPanel.Views;
    using UnityCommander.Services.Interfaces;
    using Xceed.Wpf.AvalonDock.Layout;

    /// <summary>
    /// The tab panel module.
    /// </summary>
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

        /// <summary>
        /// The on initialized.
        /// </summary>
        /// <param name="containerProvider">
        /// The container provider.
        /// </param>
        public void OnInitialized(IContainerProvider containerProvider)
        {
        }

        /// <summary>
        /// The register types.
        /// </summary>
        /// <param name="containerRegistry">
        /// The container registry.
        /// </param>
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<SplitPanelView>();
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
                    if (vm.DataContext is ITabPanel container)
                    {
                        container.InitialTabPanelContent(region.Name);
                    }
                }
            }
        }
    }
}