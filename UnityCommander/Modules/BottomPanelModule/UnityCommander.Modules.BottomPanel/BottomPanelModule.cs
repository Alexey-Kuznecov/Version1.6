
using Microsoft.Win32;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Navigation.Regions;
using UnityCommander.Common;
using UnityCommander.Core;
using UnityCommander.Modules.BottomPanel.Services;
using UnityCommander.Modules.BottomPanel.ViewModels;
using UnityCommander.Modules.BottomPanel.Views;
using UnityCommander.Mvvm.Helper;
using UnityCommander.Services;
using UnityCommander.Services.Docking;
using UnityCommander.Services.Interfaces;
using UnityCommander.Services.Interfaces.Docking;

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
            regionManager.RequestNavigate(RegionNames.RightConsoleRegion, nameof(ConsoleView));
            regionManager.RequestNavigate(RegionNames.LeftConsoleRegion, nameof(ConsoleView));
            //regionManager.RequestNavigate(RegionNames.LogTabRegion, nameof(LogView));
            regionManager.RequestNavigate(RegionNames.PreviewRegion, nameof(PreviewView));

            var coordinator =
                containerProvider.Resolve<ISessionAggregator>();

            var vm = RegionViewModelHelper.GetViewModel<BottomPanelViewModel>(
                regionManager,
                RegionNames.BottomPanelRegion);

            if (vm != null)
            {
                coordinator.RegisterCapture(vm.Capture);
                coordinator.RegisterRestore(vm.Restore);
            }
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<BottomPanelView>();
            containerRegistry.RegisterForNavigation<ConsoleView>();
            //containerRegistry.RegisterForNavigation<LogView>();
            containerRegistry.RegisterForNavigation<PreviewView>();
        }
    }
}