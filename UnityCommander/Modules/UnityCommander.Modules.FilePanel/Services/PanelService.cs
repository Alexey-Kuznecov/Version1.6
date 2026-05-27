
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using UnityCommander.Common.Commands;
using UnityCommander.Common.Docking;
using UnityCommander.Common.Module;
using UnityCommander.Common.Panels.Panels;
using UnityCommander.Core.Helper;
using UnityCommander.Logging.Configuration;
using UnityCommander.Logging.Contracts;
using UnityCommander.Logging.Core;
using UnityCommander.Logging.Infrastructure;
using UnityCommander.Modules.FilePanel.Views;
using UnityCommander.Services;
using UnityCommander.Services.Interfaces;
using UnityCommander.Services.Interfaces.Bootstrap;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout;

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
            _panelRegistry.TabAdded += _panelRegistry_TabAdded;
            _tabDoubleClick = new DoubleClickHandlerHelper(_logger);
        }

        private void _panelRegistry_TabAdded(TabAddedEvent panel)
        {
            //ProcessNavigation();
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
                        _panelRegistry.AddTab(op.ToPanelId.Value, op.TabId);
                        break;

                    case TabOperationType.Remove:
                        _panelRegistry.RemoveTab(op.TabId);
                        break;

                    case TabOperationType.Move:
                        _panelRegistry.MoveTab(op.ToPanelId.Value, op.TabId);
                        break;

                    case TabOperationType.Activate:
                        _panelRegistry.SetActiveTab(op.ToPanelId.Value, op.TabId);
                        break;
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

            _tabRegistry.SetActive(vm.GetPanelToken());

            if (_panelRegistry.FindPanelByTab(_tabRegistry.ActiveTab.TabId) is Guid panelId)
            {
                _panelRegistry.SetActivePanel(panelId);
            }
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
                if (result.Result != true) return;

                var view = result.Context.NavigationService.Region.ActiveViews
                               .FirstOrDefault() as SplitPanelView;

                var viewModel = view?.DataContext as ITabPanelContent;
                if (viewModel == null) return;

                viewModel.InitializedViewModel(ref tabId, basePath);

                //var adapter = new TabContentAdapter(viewModel);

                //_tabRegistry.Register(adapter);
                _panelRegistry.AddTab(panelId, tabId);
                _panelRegistry.SetActiveTab(panelId, tabId);
            });
        }
    }
}
