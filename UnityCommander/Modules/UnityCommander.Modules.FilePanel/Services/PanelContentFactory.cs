
using Prism.Navigation.Regions;
using System;
using System.Linq;
using System.Windows.Controls;
using UnityCommander.Abstractions.Panels;
using UnityCommander.Modules.FilePanel.Views;
using UnityCommander.Services;
using UnityCommander.WPF;

namespace UnityCommander.Modules.FilePanel.Services
{
    public class PanelContentFactory : ILayoutContentFactory
    {
        private readonly IRegionManager _regionManager;
        private readonly ITabRegistry _tabRegistry;
        private readonly ITabStateRegistry _tabStateRegistry;

        private TabContentAdapter _adapter;

        public PanelContentFactory(
            IRegionManager regionManager, 
            ITabRegistry tabRegistry, 
            IPanelRegistry panelRegistry, 
            ITabStateRegistry tabStateRegistry)
        {
            _regionManager = regionManager;
            _tabRegistry = tabRegistry;
            _tabStateRegistry = tabStateRegistry;
        }

        public void Create(
            ContentControl content,
            Guid tabId,
            Guid panelId, 
            string path, 
            Action<ITabPanelContent> onReady)
        {
            var viewMode = _tabStateRegistry.GetPanelViewMode(panelId);

            var regionName = $"Tab_{Guid.NewGuid()}";

            RegionManager.SetRegionName(content, regionName);
            RegionManager.SetRegionManager(content, _regionManager);

            _regionManager.RequestNavigate(regionName, nameof(SplitPanelView), result =>
            {
                if (!result.Success)
                    return;

                var view = result.Context.NavigationService.Region.ActiveViews
                    .FirstOrDefault() as SplitPanelView;

                var vm = view?.DataContext as ITabPanelContent;

                if (vm != null)
                {
                    vm.InitializedViewModel(ref tabId, path, panelId, viewMode);

                    _adapter = new TabContentAdapter(vm);
                    _tabRegistry.Register(_adapter);

                    onReady?.Invoke(vm);
                }
            });
        }
    }
}
