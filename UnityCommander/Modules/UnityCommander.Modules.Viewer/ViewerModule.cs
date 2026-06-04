using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using UnityCommander.Core;
using UnityCommander.Modules.Viewer.ViewModels;

namespace UnityCommander.Modules.Viewer
{
    public class ViewerModule : IModule
    {

        private readonly IRegionManager regionManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="ToolBarModule"/> class.
        /// </summary>
        /// <param name="regionManager"> The region manager. </param>
        public ViewerModule(IRegionManager regionManager)
        {
            this.regionManager = regionManager;
        }
        public void OnInitialized(IContainerProvider containerProvider)
        {
            this.regionManager.RequestNavigate(RegionNames.ViewerRegion, nameof(ViewerViewModel));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<ViewerViewModel>();
        }
    }
}