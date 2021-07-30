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
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Controls;
    using System.Windows.Documents;

    using Prism.Ioc;
    using Prism.Modularity;
    using Prism.Regions;

    using UnityCommander.Core;
    using UnityCommander.Core.Commands;
    using UnityCommander.Core.Modules;
    using UnityCommander.Modules.FilePanel.ViewModels;
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
            this.regionManager.RequestNavigate("LeftPanelRegion", "SplitPanelView");
            this.regionManager.RequestNavigate("RightPanelRegion", "SplitPanelView");
            this.regionManager.RequestNavigate(RegionNames.FilePanelRegion, "ViewA");
            this.regionManager.AddToRegion("LeftPanelRegion", new SplitPanelView());
            this.regionManager.AddToRegion("RightPanelRegion", new SplitPanelView());
            this.regionManager.AddToRegion("RightPanelRegion", new SplitPanelView());
            
            var viewsLeftPanel = this.regionManager.Regions["LeftPanelRegion"].Views.ToList();
            var viewsRightPanel = this.regionManager.Regions["RightPanelRegion"].Views.ToList();
            var views = this.regionManager.Regions[RegionNames.FilePanelRegion].Views.ToList();

            Guid[] guids = new Guid[2];
            guids[0] = Guid.NewGuid();
            guids[1] = Guid.NewGuid();

            for (int i = 0; i < 1; i++)
            {
                if (viewsLeftPanel[i] is UserControl leftPanel)
                {
                    if (leftPanel.DataContext is IDirectoryPanel directoryPanel)
                    {
                        directoryPanel.InitialPanel(guids[0]);
                    }
                }

                if (viewsRightPanel[i] is UserControl rightPanel)
                {
                    if (rightPanel.DataContext is IDirectoryPanel directoryPanel)
                    {
                        directoryPanel.InitialPanel(guids[1]);
                    }
                }

                if (views[i] is UserControl view)
                {
                    if (view.DataContext is IPanelContainer directoryPanel)
                    {
                        directoryPanel.InitialPanel(guids);
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
            containerRegistry.RegisterForNavigation<ViewA>();
            containerRegistry.RegisterForNavigation<SplitPanelView>();
        }
    }
}