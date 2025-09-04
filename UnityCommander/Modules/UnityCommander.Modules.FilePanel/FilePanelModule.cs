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
    using Prism.Ioc;
    using Prism.Modularity;
    using Prism.Regions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using UnityCommander.Common.Module;
    using UnityCommander.Modules.FilePanel.Views;
    using UnityCommander.Services.Interfaces;
    using Xceed.Wpf.AvalonDock.Layout;
    using Prism.Commands;
    using UnityCommander.Services.Interfaces.Database.Queries.Xml;
    using System.IO;
    using Xceed.Wpf.AvalonDock.Layout.Serialization;
    using Prism.Mvvm;
    using System.Windows.Threading;
    using CommandSystem.Gui.Integraion;
    using CommandSystem.Core.Commands;
    using System.Threading.Tasks;
    using CommandSystem.Core.Metadata;

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
        private GuiCommandRegistrar _commandRegistered;
        private IDockingService _dockingService;
        private IAppConfigService _appConfigService;
        HashSet<LayoutDocumentPane> knownPanes = new();
        private string _currentPath  = string.Empty;
        private string _currentCommand  = string.Empty;


        public Action<CommandContext> GetCurrentPathCommand => new Action<CommandContext>(
            (ctx) =>
            {
                _ = GetCurrentPath(ctx);
            }
        );

        public Func<CommandContext, Task> SetCurrentPathCommand => new Func<CommandContext, Task>(
            async (ctx) =>
            {
                _ = await SetCurrentPath(ctx);
            }
        );

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

                var serializer = new XmlLayoutSerializer(_dockingService.GetDockingManager());

                using (var stream = new StreamWriter("layout.xml"))
                {
                    serializer.Serialize(stream);
                }
            });

        /// <summary>
        /// Initializes a new instance of the <see cref="FilePanelModule"/> class.
        /// </summary>
        /// <param name="regionManager"> The region manager. </param>
        public FilePanelModule(IRegionManager regionManager)
        {
            this.regionManager = regionManager;
        }

        /// <summary>
        /// The on initialized.
        /// </summary>
        /// <param name="containerProvider"> The container provider. </param>
        public void OnInitialized(IContainerProvider containerProvider)
        {
            _multiCommands = containerProvider.Resolve<IMultiCommandService>();
            _commandRegistered = containerProvider.Resolve<GuiCommandRegistrar>();
            //_commandExecute = containerProvider.Resolve<GuiCommandExecute>();
            _multiCommands.SaveCommand.RegisterCommand(this.SavePanelStateCommand);
            var metadata = new CommandMetadata("getcurpath", "Получает текущий путь директории");
            this._commandRegistered.Register(metadata, GetCurrentPathCommand);
            metadata = new CommandMetadata("setcurpath", "Устанавливает текущий путь директории");
            this._commandRegistered.Register(metadata, SetCurrentPathCommand);
            _dockingService = containerProvider.Resolve<IDockingService>();
            var manager = _dockingService.GetDockingManager();
            manager.MouseDoubleClick += Manager_MouseDoubleClick;

            _appConfigService = containerProvider.Resolve<IAppConfigService>();

            // 🧠 Попробуем восстановить layout, если он есть
            var layoutFilePath = "layout.xml";
            if (File.Exists(layoutFilePath))
            {
                var serializer = new XmlLayoutSerializer(manager);

                // Привязка контента при восстановлении
                serializer.LayoutSerializationCallback += (s, args) =>
                {
                    var path = args.Model.Title;

                    if (!string.IsNullOrEmpty(path))
                    {
                        var token = Guid.NewGuid();
                        var regionName = $"Tab_{token}";
                        var contentControl = new ContentControl();


                        RegionManager.SetRegionName(contentControl, regionName);
                        ViewModelLocator.SetAutoWireViewModel(contentControl, true);

                        args.Content = contentControl;
                        Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(() =>
                        {
                            regionManager.RequestNavigate(regionName, nameof(SplitPanelView), result =>
                            {
                                if (result.Result == true)
                                {
                                    var view = result.Context.NavigationService.Region.ActiveViews.FirstOrDefault() as SplitPanelView;
                                    var viewModel = view?.DataContext as ITabPanelContent;
                                    viewModel.InitializedViewModel(ref token, path);
                                    args.Content = view; // Привязать контент
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
                    _dockingService.AddDocumentTab(config.Path, regionName);
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

        private void Manager_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (_currentCommand == "setcurpath")
            {
                _currentCommand = string.Empty;
                return; 
            }

            var path = _currentPath;
            var token = Guid.NewGuid();
            var regionName = $"Tab_{token}";
            _dockingService.AddActiveDocumentTab(path, regionName);

            regionManager.RequestNavigate(regionName, nameof(SplitPanelView), result =>
            {
                if (result.Result == true)
                {
                    var view = result.Context.NavigationService.Region.ActiveViews.FirstOrDefault() as SplitPanelView;
                    var viewModel = view?.DataContext as ITabPanelContent;
                    viewModel.InitializedViewModel(ref token, path);
                }
            });
        }

        private string GetCurrentPath(CommandContext ctx)
        {
            return _currentPath;
        }

        private async Task<string> SetCurrentPath(CommandContext ctx)
        {
            _currentPath = ctx.Parameter.ToString();
            _currentCommand = ctx.Name;

            Task<string> task = Task.Run(() =>
            {
                return ctx.Parameter.ToString();
            });

            return await task;
        }

        /// <summary>
        /// The register types.
        /// </summary>
        /// <param name="containerRegistry"> The container registry. </param>
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<SplitPanelView>();
        }
    }
}