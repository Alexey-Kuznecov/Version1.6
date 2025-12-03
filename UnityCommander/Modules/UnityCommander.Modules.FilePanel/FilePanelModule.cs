
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
    using UnityCommander.Modules.FilePanel.Views;
    using UnityCommander.Services;
    using UnityCommander.Services.Interfaces;
    using UnityCommander.Services.Interfaces.Database.Queries.Xml;
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
        private GuiCommandRegistrar _commandRegistered;
        private GuiCommandExecute _commandExecute;
        private IDockingService _dockingService;
        private IAppConfigService _appConfigService;
        private IPanelRegistry _panelRegistry;
        private string _currentCommand  = string.Empty;
        private IAppLogger _appLogger;

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
            this._appLogger = containerProvider.Resolve<IAppLogger>(); ;
            _multiCommands = containerProvider.Resolve<IMultiCommandService>();
            _panelRegistry = containerProvider.Resolve<IPanelRegistry>();
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
            manager.ActiveContentChanged += Manager_ActiveContentChanged;
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

        // поле класса — для дебаунса (вставь в класс)
        private DateTime _lastNavigationTime = DateTime.MinValue;
        private DateTime _lastDoubleClickHandled = DateTime.MinValue;

        // вызывай это при навигации (вместо CurrentChanged handler или там где делаешь Back/Forward)
        private void MarkNavigationOccured()
        {
            _lastNavigationTime = DateTime.UtcNow;
        }

        // Заменяющий метод
        private void Manager_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // дебаунс: если двойной клик пришёл сразу после навигации — игнорируем (защита от "быстро нажал назад")
                if ((DateTime.UtcNow - _lastNavigationTime).TotalMilliseconds < 300)
                {
#if (Nlog1)
                    _appLogger.Info("DoubleClick: ignored due to recent navigation");
#endif
                    return;
                }

                var dockObj = sender as DockingManager;
                if (dockObj == null) return;

                // hit test
                var pos = e.GetPosition(dockObj);
                var hit = VisualTreeHelper.HitTest(dockObj, pos);
                if (hit == null) return;

                var start = hit.VisualHit as DependencyObject;
                if (start == null) return;

                // Собираем полную цепочку для логирования (как у тебя было)
                var fullChain = new List<string>();
                var node = start;
                while (node != null)
                {
                    fullChain.Add(node.GetType().Name);
                    node = GetParentSafely(node);
                }
#if (Nlog1)
                _appLogger.Info("DoubleClick Hit chain: " + string.Join(" -> ", fullChain));
#endif
                // 1) Обязательно должна быть LayoutDocumentPaneControl в цепочке
                bool hasPane = fullChain.Any(n => n == "LayoutDocumentPaneControl");
                if (!hasPane)
                {
#if (Nlog1)
                    _appLogger.Info("DoubleClick: no LayoutDocumentPaneControl -> reject");
#endif
                    return;
                }

                // 2) Проверяем первые N узлов от VisualHit вверх — если среди них есть элементы списка/контрола, отклоняем.
                int checkDepth = 6; // количество ближайших узлов, которые анализируем
                var firstNodes = fullChain.Take(checkDepth).ToArray();

                // blacklist близких типов (на основе твоих логов)
                var closeBlacklist = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                {
                    "ListViewItem","ListView","ListBoxItem","GridViewRowPresenter","TextBlock",
                    "TreeViewItem","ScrollViewer","GridView","Path","Button","Image", "ContentControl"
                };

                if (firstNodes.Any(n => closeBlacklist.Contains(n)))
                {
#if (Nlog1)
                    _appLogger.Info("DoubleClick: close-blacklist hit -> reject (" +
                                    string.Join(", ", firstNodes.Where(n => closeBlacklist.Contains(n))) + ")");
#endif
                    return;
                }

                // 3) Если ближайшие узлы не указывают на список/контент — пропускаем.
                // Доп. эвристика: если start сам по себе является LayoutDocumentControl (то есть клик по заголовку),
                // это OK.
                if (start.GetType().Name == "LayoutDocumentControl")
                {
                    // разрешить
                }
                else
                {
                    // Дополнительно: убедимся, что клик не был глубоко внутри ContentPresenter (внутри документа)
                    int idxContentPresenter = fullChain.FindIndex(n => n == "ContentPresenter");
                    if (idxContentPresenter >= 0 && idxContentPresenter <= 2)
                    {
#if (Nlog1)
                        _appLogger.Info("DoubleClick: ContentPresenter close to hit -> reject");
#endif
                        return;
                    }
                }

                // Дополнительно: если в цепочке есть явные управляющие элементы (toolbar, menu и т.д.) — отклоняем.
                var broaderBlacklist = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                {
                    "ToolBar","ToolBarTray","MenuItem","ContextMenu","Thumb","Popup"
                };
                if (fullChain.Any(n => broaderBlacklist.Contains(n)))
                {
#if (Nlog1)
                    _appLogger.Info("DoubleClick: broader blacklist -> reject");
#endif
                    return;
                }

                // Пройдя все проверки — считаем это клик по области вкладок
                // Доп. защита: минимальный интервал между обработками double-click, чтобы не было "быстрой серии"
                if ((DateTime.UtcNow - _lastDoubleClickHandled).TotalMilliseconds < 200)
                {
#if (Nlog1)
                    _appLogger.Info("DoubleClick: suppressed due to double handling cooldown");
#endif
                    return;
                }

                _lastDoubleClickHandled = DateTime.UtcNow;

                // --- Создаём вкладку ---
                var context = _commandExecute.Execute("getcurpath");
                var basePath = context.Result as string;

                var token = Guid.NewGuid();
                var regionName = $"Tab_{token}";
                _dockingService.AddActiveDocumentTab(basePath, regionName);

                regionManager.RequestNavigate(regionName, nameof(SplitPanelView), result =>
                {
                    if (result.Result == true)
                    {
                        var view = result.Context.NavigationService.Region.ActiveViews.FirstOrDefault() as SplitPanelView;
                        var viewModel = view?.DataContext as ITabPanelContent;
                        viewModel?.InitializedViewModel(ref token, basePath);
                    }
                });
            }
            catch (Exception ex)
            {
#if (Nlog1)
                _appLogger.Info("Manager_MouseDoubleClick error: " + ex);
#endif
            }
        }

        // Вспомогательный метод — уже у тебя есть похожий, но оставлю для полноты
        private DependencyObject GetParentSafely(DependencyObject node)
        {
            if (node == null) return null;
            if (node is Visual || node is Visual3D)
                return VisualTreeHelper.GetParent(node);
            return LogicalTreeHelper.GetParent(node) as DependencyObject;
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
    }
}