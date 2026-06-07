using Prism.Ioc;
using Prism.Modularity;
using Prism.Navigation.Regions;
using UnityCommander.Core;
using UnityCommander.Modules.WebBrowser.Views;

namespace UnityCommander.Modules.WebBrowser
{
    public class WebBrowserModule : IModule
    {
        /// <summary>
        /// The region manager.
        /// </summary>
        private readonly IRegionManager regionManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebBrowserModule"/> class.
        /// </summary>
        /// <param name="regionManager"> The region manager. </param>
        public WebBrowserModule(IRegionManager regionManager)
        {
            this.regionManager = regionManager;
        }

        /// <summary>
        /// The on initialized.
        /// </summary>
        /// <param name="containerProvider"> The container provider. </param>
        public void OnInitialized(IContainerProvider containerProvider)
        {
            this.regionManager.RequestNavigate(RegionNames.WebBrowserRegion, "WebBrowserView");
        }

        /// <summary>
        /// The register types.
        /// </summary>
        /// <param name="containerRegistry"> The container registry. </param>
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<WebBrowserView>();
        }
    }
}