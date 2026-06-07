

using Prism.Navigation.Regions;
using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using UnityCommander.Common.Docking;
using UnityCommander.Common.Module;
using UnityCommander.Common.Panels;
using UnityCommander.Core.Helper;
using UnityCommander.Logging.Configuration;
using UnityCommander.Logging.Core;
using UnityCommander.Logging.Infrastructure;
using UnityCommander.Modules.FilePanel.Views;
using UnityCommander.Services;
using UnityCommander.Services.Interfaces;
using UnityCommander.Services.Interfaces.Bootstrap;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout;
using ILogger = UnityCommander.Logging.Contracts.ILogger;

namespace UnityCommander.Modules.FilePanel.Services
{
    public class PanelService : IPanelService
    {
        private readonly ILogger _logger;
        private readonly IRegionManager _regionManager;
        private readonly IDockingService _dockingService;
        private readonly IPanelRegistry _panelRegistry;
        private readonly ITabRegistry _tabRegistry;
        private readonly IDockingSyncService _dockingSyncService;
        private DoubleClickHandlerHelper _tabDoubleClick;
        private CommandService _command;
        private DateTime _lastNavigationTime = DateTime.MinValue;

        public PanelService(
            IRegionManager regionManager,
            IDockingService dockingService,
            IPanelRegistry panelRegistry,
            ITabRegistry tabRegistry,
            IDockingSyncService dockingSyncService,
            CommandService command,
            LoggerCreator logger)
        {
            var log = logger;
            _logger = log.Create(
               category: LogCategory.UserAction,
               scope: LogScope.Runtime
            );

            _regionManager = regionManager;
            _dockingService = dockingService;
            _tabRegistry = tabRegistry;
            _panelRegistry = panelRegistry;
            _dockingSyncService = dockingSyncService;
            _command = command;
        }

        public void Initialize()
        {
            var manager = _dockingService.GetDockingManager();
            manager.MouseDoubleClick += Manager_MouseDoubleClick;
            manager.ActiveContentChanged += Manager_ActiveContentChanged;
            _dockingSyncService.OnDiff += _dockingSyncService_OnDiff;
            _dockingSyncService.OnDocumentClose += _dockingSyncService_OnDocumentClose;
            _dockingSyncService.OnFloatingWindow += _dockingSyncService_OnFloatingWindow; ;
            _tabDoubleClick = new DoubleClickHandlerHelper(_logger);
        }

        private void _dockingSyncService_OnFloatingWindow(object obj)
        {
            //throw new NotImplementedException();
        }

        private void _dockingSyncService_OnDocumentClose(object obj)
        {
            foreach (var item in _tabRegistry.GetAllTabs())
            {
                if (!_panelRegistry.Contains(item.TabId))
                    _tabRegistry.Unregister(item.TabId);
            }
        }

        private void _dockingSyncService_OnDiff(DiffResult result)
        {
            if (result == null) 
                return;

            foreach (var op in result.Operations)
            {
                switch (op.Type)
                {
                    case TabOperationType.Add:
                        
                        var before = _panelRegistry.GetDebugState();

                        var activeTab = _tabRegistry.ActiveTab;

                        _panelRegistry.AddTab(op.ToPanelId.Value, op.TabId);

                        var state = _panelRegistry.GetDebugState();

                        _logger.Debug($"REMOVE: {op.TabId} => {activeTab.TabId}");
                        _logger.Debug($"Panels: {before.PanelCount} => {state.PanelCount}");
                        _logger.Debug($"Tabs: {before.TabCount} => {state.TabCount}");

                        break;

                    case TabOperationType.Remove:
                        
                        before = _panelRegistry.GetDebugState();

                        activeTab = _tabRegistry.ActiveTab;

                        _panelRegistry.RemoveTab(op.TabId);

                        state = _panelRegistry.GetDebugState();

                        _logger.Debug($"REMOVE: {op.TabId} => {activeTab.TabId}");
                        _logger.Debug($"Panels: {before.PanelCount} => {state.PanelCount}");
                        _logger.Debug($"Tabs: {before.TabCount} => {state.TabCount}");

                        if (op.FromPanelId is Guid panelId)
                        {
                            if (_panelRegistry.IsEmpty(panelId))
                                _panelRegistry.RemovePanel(panelId);
                        }

                        break;

                    case TabOperationType.Move:

                        //foreach (var item in _tabRegistry.GetAllTabs())
                        //{
                        //    if (!_panelRegistry.Contains(item.TabId))
                        //        _tabRegistry.Unregister(item.TabId);
                        //}

                        break;

                    case TabOperationType.Activate:
                        _panelRegistry.SetActiveTab(op.ToPanelId.Value, op.TabId);
                        break;
                }
            }
        }

        private void SynchronizeTabs()
         {
             foreach (var tab in _tabRegistry.GetAllTabs())
             {
                 if (!_panelRegistry.Contains(tab.TabId))
                 {
                     _tabRegistry.Unregister(tab.TabId);
                 }
             }
         }

        private void Manager_ActiveContentChanged(object sender, EventArgs e)
        {
            if (sender is not DockingManager
                {
                    ActiveContent: LayoutDocument
                    {
                        Content: ContentControl
                        {
                            Content: ContentControl
                            {
                                DataContext: ITabPanelContent vm
                            }
                        }
                    }
                })
            {
                return;
            }

            if (vm == null)
                return;

            var tabId = vm.GetPanelToken();

            if (!_tabRegistry.Contains(tabId))
                return;

            if (_panelRegistry.FindPanelByTab(tabId) is not Guid panelId)
                return;

            _panelRegistry.SetActiveTab(panelId, tabId);
            _tabRegistry.SetActive(tabId);

            _logger.Debug($"Total tabs: {_tabRegistry.GetAllTabs().Count}");
            _logger.Debug($"Total panels: {_panelRegistry.GetAllPanels().Count}");
            _logger.Debug($"Activate tab: {_tabRegistry.ActiveTab.GetCurrentPath()}");
            _logger.Debug($"Token: {_tabRegistry.ActiveTab.TabId}");
        }

        private void Manager_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var dockObj = sender as DockingManager;
                if (dockObj == null) return;

                var pos = e.GetPosition(dockObj);
                var hit = VisualTreeHelper.HitTest(dockObj, pos);
                if (hit == null) return;

                var result = _tabDoubleClick.HandleDoubleClick(dockObj, hit.VisualHit, _lastNavigationTime);

                if (result)
                {
                    ProcessNavigation();
                }
            }
            catch (Exception ex)
            {
                _logger.Info("Manager_MouseDoubleClick error: " + ex);
            }
        }

        private void ProcessNavigation()
        {
            var basePath = _tabRegistry.ActiveTab.GetCurrentPath();

            var panel = _panelRegistry.GetActivePanel(); // Панель пустая 
            var panelId = panel.PanelId;

            var tabId = Guid.NewGuid();
            var tabHeader = PathTitleHelper.GetTabTitle(basePath);
            var regionName = $"Tab_{tabId}";

            _dockingService.AddActiveDocumentTab(tabId.ToString(), tabHeader, regionName);

            _regionManager.RequestNavigate(regionName, nameof(SplitPanelView), result =>
            {
                if (!result.Success)
                    return;

                var view = result.Context.NavigationService.Region.ActiveViews
                               .FirstOrDefault() as SplitPanelView;

                var viewModel = view?.DataContext as ITabPanelContent;
                if (viewModel == null) return;

                viewModel.InitializedViewModel(ref tabId, basePath);

                var adapter = new TabContentAdapter(viewModel);
                _tabRegistry.Register(adapter);

                //_panelRegistry.AddTab(panelId, tabId);
                //_panelRegistry.SetActiveTab(panelId, tabId);
            });
        }
    }
}
