using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using UnityCommander.Core;
using UnityCommander.Modules.SettingsPanel.Views;

namespace UnityCommander.Modules.SettingsPanel
{
    public class SettingsPanelModule : IModule
    {

        /// <summary>
        /// The region manager.
        /// </summary>
        private readonly IRegionManager regionManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsPanelModule"/> class.
        /// </summary>
        /// <param name="regionManager"> The region manager. </param>
        public SettingsPanelModule(IRegionManager regionManager)
        {
            this.regionManager = regionManager;
        }

        /// <summary>
        /// The on initialized.
        /// </summary>
        /// <param name="containerProvider"> The container provider. </param>
        public void OnInitialized(IContainerProvider containerProvider)
        {
            this.regionManager.RequestNavigate(RegionNames.SettingsPanelRegion, "SettingsPanelView");
        }

        /// <summary>
        /// The register types.
        /// </summary>
        /// <param name="containerRegistry"> The container registry. </param>
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<SettingsPanelView>();
        }
    }
}