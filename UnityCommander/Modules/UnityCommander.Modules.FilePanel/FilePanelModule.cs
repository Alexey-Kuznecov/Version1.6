// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FilePanelModule.cs" company="T">
// Copyright (p) Alexey Kuznecov. All right reserved.
// </copyright>
// <summary>
//   The file panel module.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace UnityCommander.Modules.FilePanel
{
    using System;
    using System.Windows;

    using Prism.Ioc;
    using Prism.Modularity;
    using Prism.Regions;

    using UnityCommander.Core;
    using UnityCommander.Core.Modules;
    using UnityCommander.Modules.FilePanel.Views;

    /// <summary>
    /// The file panel module.
    /// </summary>
    public class FilePanelModule : IModule
    {
        /// <summary>
        /// The _region manager.
        /// </summary>
        private IRegionManager regionManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilePanelModule"/> class.
        /// </summary>
        /// <param name="regionManager"> The region manager. </param>
        public FilePanelModule(IRegionManager regionManager)
        {
            this.regionManager = regionManager;
        }

        /// <summary>
        /// The on initialized.
        /// </summary>
        /// <param name="containerProvider"> The container provider. </param>
        public void OnInitialized(IContainerProvider containerProvider)
        {
            this.regionManager.RequestNavigate(RegionNames.FilePanelRegion, nameof(MainView));
            this.regionManager.RequestNavigate(NestedRegionNames.LeftFilePanelRegion, nameof(ViewA), this.NavigationCallback);
            this.regionManager.RequestNavigate(NestedRegionNames.RightFilePanelRegion, nameof(ViewB), this.NavigationCallback);
        }

        /// <summary>
        /// The register types.
        /// </summary>
        /// <param name="containerRegistry"> The container registry. </param>
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<MainView>();
            containerRegistry.RegisterForNavigation<ViewA>();
            containerRegistry.RegisterForNavigation<ViewB>();
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
                    if (vm.DataContext is IPanelContainer container)
                    {
                        container.InitialDirectoryPanel(region.Name);
                    }
                }
            }
        }
    }
}