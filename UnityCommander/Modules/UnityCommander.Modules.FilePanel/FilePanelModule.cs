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
        private SplitPanelView leftPanelContent;
        private SplitPanelView rightPanelContent;

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
            this.leftPanelContent = new SplitPanelView();
            this.rightPanelContent = new SplitPanelView();
            var parameter1 = new NavigationParameters { { "viewmodel", this.leftPanelContent } };
            var parameter2 = new NavigationParameters { { "viewmodel", this.rightPanelContent } };

            this.regionManager.RequestNavigate(RegionNames.FilePanelRegion, nameof(MainView));
            this.regionManager.RequestNavigate(NestedRegionNames.LeftFilePanelRegion, nameof(ViewA), this.NavigationCallback, parameter1);
            this.regionManager.RequestNavigate(NestedRegionNames.RightFilePanelRegion, nameof(ViewB), this.NavigationCallback, parameter2);
            this.regionManager.RegisterViewWithRegion(NestedRegionNames.LeftPanelContentRegion, this.GetLeftPanelContent);
            this.regionManager.RegisterViewWithRegion(NestedRegionNames.RightPanelContentRegion, this.GetRightPanelContent);
        }

        private object GetLeftPanelContent()
        {
            return this.leftPanelContent;
        }

        private object GetRightPanelContent()
        {
            return this.rightPanelContent;
        }

        private void NavigationCallback(NavigationResult obj)
        {
            var region = obj.Context.NavigationService.Region;
            var viewModel = obj.Context.Parameters;

            foreach (var view in region.Views)
            {
                if (view is FrameworkElement { DataContext: IPanelContainer container })
                {
                    if (((FrameworkElement)viewModel["viewmodel"]).DataContext is IDirectoryPanel directoryPanel)
                    {
                        directoryPanel.InitializedViewModel();
                        container.InitialDirectoryPanel(directoryPanel, region.Name);
                    }
                }
            }
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
    }
}