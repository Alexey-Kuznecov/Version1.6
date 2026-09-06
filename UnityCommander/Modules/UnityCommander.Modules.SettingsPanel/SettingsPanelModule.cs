
using Prism.Ioc;
using Prism.Modularity;
using Prism.Navigation.Regions;

namespace UnityCommander.Modules.SettingsPanel
{
    public class SettingsPanelModule : IModule
    {
        private readonly IRegionManager regionManager;

        public SettingsPanelModule(IRegionManager regionManager)
        {
            this.regionManager = regionManager;
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            //containerRegistry.RegisterForNavigation<SettingsPanelView>();
        }
    }
}