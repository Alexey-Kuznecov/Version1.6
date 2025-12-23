
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FilePanelModule.cs" company="T">
// Copyright (p) Alexey Kuznecov. All right reserved.
// </copyright>
// <summary>
//   The file panel module.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace UnityCommander.Modules.FilePanel
{
    using CommandSystem.Core.Commands;
    using CommandSystem.Core.Metadata;
    using CommandSystem.Gui.Integraion;
    using Prism.Commands;
    using Prism.Ioc;
    using Prism.Modularity;
    using Prism.Mvvm;
    using Prism.Regions;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;
    using System.Windows.Threading;
    using UnityCommander.Common.Module;
    using UnityCommander.Core.Commands.Base;
    using UnityCommander.Logging.Abstractions;
    using UnityCommander.Modules.FilePanel.Views;
    using UnityCommander.Services;
    using UnityCommander.Services.Interfaces;
    using UnityCommander.Services.Interfaces.Database.Queries.Xml;
    using UnityCommander.Services.Interfaces.Settings;
    using Xceed.Wpf.AvalonDock;
    using Xceed.Wpf.AvalonDock.Layout;
    using Xceed.Wpf.AvalonDock.Layout.Serialization;

    /// <summary>
    /// The file panel module.
    /// </summary>
    public class FilePanelModule : IModule
    {
        /// <summary>
        /// The _region manager.
        /// </summary>
        private readonly IRegionManager regionManager;
        private IMultiCommandService _multiCommands;
        private IDockingService _dockingService;
        private IAppConfigService _appConfigService;
        private IPanelRegistry _panelRegistry;
        private ILogger _logger;
        private DoubleClickHandlerHelper _doubleClickHelper;

        // поле класса — для дебаунса (вставь в класс)
        private DateTime _lastNavigationTime = DateTime.MinValue;

        public FilePanelModule(IRegionManager regionManager)
        {
            this.regionManager = regionManager;
        }

        public DelegateCommand SavePanelStateCommand => new DelegateCommand(
            () =>
            {
                var appConfig = _appConfigService.GetSession();
                var tabs = appConfig.Find("Tabs").ToList();
                var tabsResult = tabs.Single(tab => tab.ParentInfo.GetAttributeValueByName("Name") == "RightFilePanelRegion");
                tabsResult.RemoveAll();

                foreach (IRegion region in this.regionManager.Regions.Where(r => r.Name.Contains("Tab")))
                {
                    foreach (var view in region.Views)
                    {
                        if (view is FrameworkElement { DataContext: ITabPanelContent panelContent })
                        {
                            tabsResult.Add(
                                elementRecord =>
                                {
                                    elementRecord.Tag = "Tab";
                                    elementRecord.Attributes.Add("Id", "{" + panelContent.GetPanelToken() + "}");
                                    elementRecord.Attributes.Add("Path", panelContent.GetCurrentPath());
                                    elementRecord.Attributes.Add("ViewType", panelContent.GetType().Name);
                                    return elementRecord;
                                });
                        }
                    }
                }

                appConfig.Save();
                var dock = _dockingService.GetDockingManager();
                var serializer = new XmlLayoutSerializer(_dockingService.GetDockingManager());

                using (var stream = new StreamWriter("layout.xml"))
                {
                    serializer.Serialize(stream);
                }
            });

        public void OnInitialized(IContainerProvider containerProvider)
        {
            _logger = containerProvider.Resolve<ILogger>(); ;
            _multiCommands = containerProvider.Resolve<IMultiCommandService>();
            _panelRegistry = containerProvider.Resolve<IPanelRegistry>();
            _dockingService = containerProvider.Resolve<IDockingService>();
            _appConfigService = containerProvider.Resolve<IAppConfigService>();
            
            var layoutFilePath = "layout.xml";
            var manager = _dockingService.GetDockingManager();
            var command = containerProvider.Resolve<CommandService>();
            manager.MouseDoubleClick += Manager_MouseDoubleClick;
            manager.ActiveContentChanged += Manager_ActiveContentChanged;
            _multiCommands.SaveCommand.RegisterCommand(this.SavePanelStateCommand);
            _doubleClickHelper = new DoubleClickHandlerHelper(this._logger, command, _dockingService, regionManager);
            if (File.Exists(layoutFilePath))
            {
                var serializer = new XmlLayoutSerializer(manager);
                serializer.LayoutSerializationCallback += (s, args) =>
                {
                    var path = args.Model.ContentId;
                    args.Model.Title = PathTitleHelper.GetTabTitle(path); // красивый заголовок

                    if (!string.IsNullOrEmpty(path))
                    {
                        var token = Guid.NewGuid();
                        var regionName = $"Tab_{token}";
                        var contentControl = new ContentControl();
                        args.Content = contentControl;

                        RegionManager.SetRegionName(contentControl, regionName);
                        ViewModelLocator.SetAutoWireViewModel(contentControl, true);

                        Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(() =>
                        {
                            regionManager.RequestNavigate(regionName, nameof(SplitPanelView), result =>
                            {
                                if (result.Result == true)
                                {
                                    var view = result.Context.NavigationService.Region.ActiveViews.FirstOrDefault() as SplitPanelView;
                                    var viewModel = view?.DataContext as ITabPanelContent;
                                    viewModel.InitializedViewModel(ref token, path);
                                    args.Content = view;
                                    contentControl.LayoutUpdated += (s2, e2) =>
                                    {
                                        viewModel.TabTitleChanged += formatPath =>
                                        {
                                            args.Model.Title = formatPath;
                                        };
                                    };                                
                                }
                            });
                        }));
                    }
                };

                using (var reader = new StreamReader(layoutFilePath))
                {
                    serializer.Deserialize(reader);
                }
                var region = this.regionManager.Regions;
            }
            else
            {
                // 🧱 Если layout отсутствует — fallback к ручному восстановлению вкладок
                foreach (var config in _appConfigService.GetSession().GetTabConfigs("RightFilePanelRegion"))
                {
                    var token = config.Token;
                    var regionName = $"Tab_{Guid.NewGuid()}";
                    _dockingService.AddDocumentTab(config.Path, config.Path, regionName);
                    var region = this.regionManager.Regions.Select(r => r.Name == regionName).FirstOrDefault();
                    regionManager.RequestNavigate(regionName, nameof(SplitPanelView), result =>
                    {
                        if (result.Result == true)
                        {
                            var view = result.Context.NavigationService.Region.ActiveViews.FirstOrDefault() as SplitPanelView;
                            var viewModel = view?.DataContext as ITabPanelContent;
                            viewModel.InitializedViewModel(ref token, config.Path);
                        }
                    });
                }
            }
        }

        private void Manager_ActiveContentChanged(object? sender, EventArgs e)
        {
            var manager = sender as DockingManager;
            if (manager?.ActiveContent is LayoutDocument document)
            {
                if (document?.Content is ContentControl contol)
                {
                    if (contol?.Content is ContentControl view)
                    {
                        if (view?.DataContext is ITabPanelContent vm)
                        {
                            _panelRegistry.SetActivePanel(vm.GetPanelToken().ToString());
                        }
                    }
                }
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

                _doubleClickHelper.HandleDoubleClick(dockObj, hit.VisualHit, _lastNavigationTime);
            }
            catch (Exception ex)
            {
                _logger.Info("Manager_MouseDoubleClick error: " + ex);
            }
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<SplitPanelView>();
        }
    }
}