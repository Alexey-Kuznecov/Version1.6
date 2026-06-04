
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using UnityCommander.Autocomplete.Completion;
using UnityCommander.Autocomplete.Completion.Providers;
using UnityCommander.Autocomplete.Tokenization;
using UnityCommander.CLI.Core;
using UnityCommander.CLI.Integration;
using UnityCommander.CLI.Integration.UnityCommander.CLI.Integration;
using UnityCommander.Core;
using UnityCommander.Modules.BottomPanel.ViewModels;
using UnityCommander.Modules.BottomPanel.Views;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.Modules.BottomPanel
{
    public class BottomPanelModule : IModule
    {
        private readonly IRegionManager regionManager;

        public BottomPanelModule(IRegionManager regionManager)
        {
            this.regionManager = regionManager;
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            regionManager.RequestNavigate(RegionNames.BottomPanelRegion, nameof(BottomPanelView));
            regionManager.RequestNavigate(RegionNames.ConsoleTabRegion, nameof(ConsoleView));
            regionManager.RequestNavigate(RegionNames.LogTabRegion, nameof(LogView));
            regionManager.RequestNavigate(RegionNames.PreviewRegion, nameof(PreviewView));

            var coordinator =
                containerProvider.Resolve<ISessionAggregator>();
            var vm =
                containerProvider.Resolve<BottomPanelViewModel>();

            coordinator.RegisterCapture(vm.Capture);
            coordinator.RegisterRestore(vm.Restore);
        }


        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<BottomPanelView>();
            containerRegistry.RegisterForNavigation<ConsoleView>();
            containerRegistry.RegisterForNavigation<LogView>();
            containerRegistry.RegisterForNavigation<PreviewView>();
        }
    }
}