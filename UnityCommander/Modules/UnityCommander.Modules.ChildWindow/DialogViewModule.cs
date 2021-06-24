
namespace UnityCommander.Modules.ChildWindow
{
    using Prism.Ioc;
    using Prism.Modularity;
    using Prism.Regions;
    using UnityCommander.Core;
    using UnityCommander.Modules.ChildWindow.Views;

    /// <summary>
    /// The child window module.
    /// </summary>
    public class DialogViewModule : IModule
    {
        /// <summary>
        /// The _region manager.
        /// </summary>
        private readonly IRegionManager regionManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="DialogViewModule"/> class.
        /// </summary>
        /// <param name="regionManager"> The region manager. </param>
        public DialogViewModule(IRegionManager regionManager)
        {
           // this.regionManager = regionManager;
        }

        /// <summary>
        /// The on initialized.
        /// </summary>
        /// <param name="containerProvider"> The container provider. </param>
        public void OnInitialized(IContainerProvider containerProvider)
        {
           // this.regionManager.RequestNavigate(RegionNames.ChildWindowRegion, "DialogView");
        }

        /// <summary>
        /// The register types.
        /// </summary>
        /// <param name="containerRegistry"> The container registry. </param>
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<DialogView>();
        }
    }
}