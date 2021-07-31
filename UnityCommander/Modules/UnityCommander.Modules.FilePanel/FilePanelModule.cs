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
    using System.Collections.Generic;
    using System.Linq;
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
            this.regionManager.RequestNavigate(RegionNames.FilePanelRegion, nameof(ViewA));
            this.regionManager.RequestNavigate(NestedRegionNames.LeftPanelRegion, nameof(SplitPanelView));
            this.regionManager.RequestNavigate(NestedRegionNames.RightPanelRegion, nameof(SplitPanelView));
            this.InitialPanelRegion();
        }

        /// <summary>
        /// The register types.
        /// </summary>
        /// <param name="containerRegistry"> The container registry. </param>
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<ViewA>();
            containerRegistry.RegisterForNavigation<SplitPanelView>();
        }

        /// <summary>
        /// The initial panel region.
        /// </summary>
        private void InitialPanelRegion()
        {
            var filePanelRegion = from region in this.regionManager.Regions
                                  where region.Name == RegionNames.FilePanelRegion
                                  select region.Views;

            var panelRegion = filePanelRegion as IViewsCollection[] ?? filePanelRegion.ToArray();

            foreach (var dir in this.InitialNestedPanelRegion())
            {
                foreach (var views in panelRegion)
                {
                    foreach (var view in views)
                    {
                        if (view is FrameworkElement { DataContext: IPanelContainer panelContainer })
                        {
                            panelContainer.InitialDirectoryPanel(dir);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The initial nested panel region.
        /// </summary>
        /// <returns>
        /// The collection of the <see cref="IDirectoryPanel"/> types.
        /// </returns>
        private List<IDirectoryPanel> InitialNestedPanelRegion()
        {
            var nestedPanelRegion =
                from region in this.regionManager.Regions
                where region.Name == NestedRegionNames.RightPanelRegion || region.Name == NestedRegionNames.LeftPanelRegion
                select region.Views;

            var dirs = new List<IDirectoryPanel>();

            foreach (var views in nestedPanelRegion.ToList())
            {
                foreach (var view in views)
                {
                    if (view is FrameworkElement { DataContext: IDirectoryPanel directoryPanel })
                    {
                        dirs.Add(directoryPanel);
                        directoryPanel.InitializedViewModel();
                    }
                }
            }

            return dirs;
        }
    }
}