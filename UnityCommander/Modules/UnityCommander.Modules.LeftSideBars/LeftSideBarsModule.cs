
using System;

namespace UnityCommander.Modules.LeftSideBars
{
    using Prism.Ioc;
    using Prism.Modularity;
    using Prism.Regions;
    using UnityCommander.Core;
    using UnityCommander.Modules.LeftSideBars.Views;
    using UnityCommander.Services.Interfaces;
    using Xceed.Wpf.AvalonDock.Layout;

    /// <summary>
    /// The left side bars module.
    /// </summary>
    public class LeftSideBarsModule : IModule
    {
        /// <summary>
        /// The region manager.
        /// </summary>
        private readonly IRegionManager regionManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="LeftSideBarsModule"/> class.
        /// </summary>
        /// <param name="regionManager"> The region manager. </param>
        public LeftSideBarsModule(IRegionManager regionManager)
        {
            this.regionManager = regionManager;
        }

        /// <summary>
        /// The on initialized.
        /// </summary>
        /// <param name="containerProvider"> The container provider. </param>
        public void OnInitialized(IContainerProvider containerProvider)
        {
            this.regionManager.RequestNavigate(RegionNames.LeftSideBarRegion, nameof(Sidebar));
        }

        /// <summary>
        /// The register types.
        /// </summary>
        /// <param name="containerRegistry"> The container registry. </param>
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<Sidebar>();
        }
    }
}