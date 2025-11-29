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
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Threading;
    using UnityCommander.Common.Module;
    using UnityCommander.Modules.FilePanel.Views;
    using UnityCommander.Services.Interfaces;
    using UnityCommander.Services.Interfaces.Database.Queries.Xml;
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
        private GuiCommandRegistrar _commandRegistered;
        private GuiCommandExecute _commandExecute;
        private IDockingService _dockingService;
        private IAppConfigService _appConfigService;
        private string _currentCommand  = string.Empty;


        public Action<CommandContext> GetCurrentPathCommand => new Action<CommandContext>(
            (ctx) =>
            {
                GetCurrentPath(ctx);
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
            _commandExecute = containerProvider.Resolve<GuiCommandExecute>();
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

                                    contentControl.LayoutUpdated += (s2, e2) =>
                                    {
                                        viewModel.PathChanged += newPath =>
                                        {
                                            args.Model.Title = newPath;
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

        private void Manager_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // если команда setcurpath была вызвана — можно обработать логику
            if (_currentCommand == "setcurpath")
            {
                _currentCommand = string.Empty;
                return;
            }

            // 1. Получаем реальный источник клика
            if (e.OriginalSource is DependencyObject source)
            {
                // 2. Проверяем — не был ли клик внутри UI панели директорий
                if (FindParentOfType<ListView>(source) != null      // если список директорий — ListView
                    || FindParentOfType<TreeView>(source) != null    // если дерево директорий
                    || FindParentOfType<GridViewColumnHeader>(source) != null) // клик по заголовку таблицы
                {
                    return; // 👉 это не пустая область; вкладку НЕ СОЗДАЁМ
                }
            }

            // Получаем путь, из которого создавать новую вкладку
            var context = _commandExecute.Execute("getcurpath");
            var basePath = context.Result as string;  // или null, если нет

            var token = Guid.NewGuid();
            var regionName = $"Tab_{token}";
            _dockingService.AddActiveDocumentTab(basePath, regionName);
            regionManager.RequestNavigate(regionName, nameof(SplitPanelView), result =>
            {
                if (result.Result == true)
                {
                    var view = result.Context.NavigationService.Region.ActiveViews.FirstOrDefault() as SplitPanelView;
                    var viewModel = view?.DataContext as ITabPanelContent;
                    viewModel.InitializedViewModel(ref token, basePath);
                }
            });
        }

        private void GetCurrentPath(CommandContext ctx)
        {
            ctx.Result = _dockingService.GetActiveTabPath();
        }

        private async Task<string> SetCurrentPath(CommandContext ctx)
        {
            var value = ctx.Parameter.ToString();
            // задаём путь в активной вкладке, если она есть
            var vm = _dockingService.GetActiveDirectoryPanel() as ITabPanelContent;
            if (vm != null)
            {
                vm.SetCurrentPath(value); // предположим что в ITabPanelContent есть SetCurrentPath
            }
            // сохраняем локально команду, если нужно логировать:
            _currentCommand = ctx.Name;
            return await Task.FromResult(value);
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<SplitPanelView>();
        }

        public static T? FindParentOfType<T>(DependencyObject child) where T : DependencyObject
        {
            while (child != null)
            {
                if (child is T found)
                    return found;

                child = VisualTreeHelper.GetParent(child);
            }
            return null;
        }
    }
}