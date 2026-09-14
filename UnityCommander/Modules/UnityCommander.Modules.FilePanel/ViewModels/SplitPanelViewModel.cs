// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SplitPanelViewModel.cs" company="T">
//   Copyright (c) Alexei Kuznecov. All rights reserved.
// </copyright>
// <summary>
//   Реализация ViewModel для левой панели файлового менеджера. 
//   Обрабатывает навигацию, перетаскивание (drag & drop), работу с плагинами и обновление колонок.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Prism.Commands;
using Prism.Dialogs;
using Prism.Navigation.Regions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using UnityCommander.Abstractions.Columns;
using UnityCommander.Abstractions.Panels;
using UnityCommander.Common.Commands;
using UnityCommander.Common.Diagnostic;
using UnityCommander.Common.Models.Directory;
using UnityCommander.Common.Panels;
using UnityCommander.Controls.Layout;
using UnityCommander.Core;
using UnityCommander.Core.Helper;
using UnityCommander.Core.Mvvm;
using UnityCommander.Core.Navigation;
using UnityCommander.Diagnostics.Performance;
using UnityCommander.Logging;
using UnityCommander.Logging.Configuration;
using UnityCommander.Logging.Contracts;
using UnityCommander.Logging.Core;
using UnityCommander.Logging.Infrastructure;
using UnityCommander.Modules.FilePanel.Columns;
using UnityCommander.Modules.FilePanel.Controllers;
using UnityCommander.Modules.FilePanel.Layout;
using UnityCommander.Modules.FilePanel.States;
using UnityCommander.Services;
using UnityCommander.Services.Interfaces;
using UnityCommander.Settings.Abstactions;

namespace UnityCommander.Modules.FilePanel.ViewModels
{
    /// <summary>
    /// Представляет ViewModel левой панели файлового менеджера.
    /// Реализует обработку навигации, команд, drag & drop и интеграцию плагинных колонок.
    /// </summary>
    [Serializable]
    public class SplitPanelViewModel : RegionViewModelBase, IDirectoryPanel
    {
        private readonly FilePanelLayoutController _layoutController;
        private readonly FilePanelLayoutService _panelLayoutService;
        #region Поля и зависимости

        // --- Зависимости через DI
        private readonly IDataProviderService _dataService;
        private readonly FilePanelRuntime _panelRuntime;
        private readonly NavigationManager _navigationService;
        private readonly INavigationRegistry _navigationRegistry;
        private readonly ILogger _logger;
        private ISelectionManager _selectionManager;
        private bool _refreshScheduled = false;

        private CommandExecutionService _commandService;
        private readonly ISelectionService _selectionService;
        private readonly IDiagnosticRegistry _diagnosticRegistry;
        private readonly IColumnStateManager _columnStateManager;
        private readonly IColumnRegistry _columnRegistry;
        private readonly NodeContextRegistry _contextRegistry;
        private readonly ITabStateRegistry _tabStateRegistry;
        private readonly TabState _state;

        public event Action<string> PathChanged;
        public event Action<string> TabTitleChanged;

        private FileNodeContext _fileNodeContext;
        private DriveNodeContext _driveNodeContext;
        private FolderNodeContext _folderNodeContext;
        private UnifiedNodeContext _unifiedNodeContext;

        private FilePanelNodeSet _nodeSet;

        private Workspace _workspace;
        private WorkspaceController _workspaceController;
        private readonly IPerformanceProfiler _performanceProfiler;

        #endregion

        #region Конструктор

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="SplitPanelViewModel"/>.
        /// </summary>
        /// <param name="dialogService">Сервис для отображения диалоговых окон.</param>
        /// <param name="regionManager">Менеджер регионов Prism.</param>
        /// <param name="settingsService">Сервис для доступа к настройкам приложения.</param>
        /// <param name="dataService">Сервис для получения данных о файловой системе.</param>
        /// <param name="multiCommandService">Сервис для работы с составными командами.</param>
        /// <param name="pluginService">Сервис загрузки плагинов.</param>
        /// <param name="globalCommandService">Сервис глобальных команд.</param>
        /// <param name="iconProvider">Сервис для получения иконок.</param>
        /// <param name="configService">Сервис конфигурации приложения.</param>
        /// <param name="manager">Менеджер команд.</param>
        /// <param name="logger">Логгер для записи событий.</param>
        public SplitPanelViewModel(
              IDialogService dialogService,
              IRegionManager regionManager,
              IDataProviderService dataService,
              IDirectoryChangeNotifier directoryChangeNotifier,
              ISelectionManager selectionManager,
              CommandManager manager,
              IColumnProvider columnProvider,
              IColumnStateManager columnStateManager,
              IColumnRegistry columnRegistry,
              LoggerCreator loggerCreator,
              CommandExecutionService commandService, 
              NodeContextRegistry contextRegistry, 
              ISettingsService settingsService,
              IPerformanceProfiler profiler, 
              ITabStateRegistry tabStateRegistry,
              INavigationRegistry navigationRegistry,
              ISelectionService selectionService,
              IDiagnosticRegistry diagnosticRegistry,
              FilePanelLayoutService panelLayoutService, 
              FilePanelContextFactory panelContextFactory,
              TabState tabState)
            : base(regionManager)
        {
            _panelLayoutService = panelLayoutService;

            _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));

            _navigationRegistry = navigationRegistry ?? throw new ArgumentNullException(nameof(navigationRegistry));

            _diagnosticRegistry = diagnosticRegistry ?? throw new ArgumentNullException(nameof(diagnosticRegistry));

            _performanceProfiler = profiler;

            _contextRegistry = contextRegistry ?? throw new ArgumentNullException(nameof(contextRegistry));

            _state = tabState ?? throw new ArgumentNullException(nameof(tabState));

            _selectionService = selectionService ?? throw new ArgumentNullException(nameof(selectionService));

            _tabStateRegistry = tabStateRegistry ?? throw new ArgumentNullException(nameof(tabStateRegistry));

            _commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            _selectionManager = selectionManager ?? throw new ArgumentNullException(nameof(selectionManager));

            _columnStateManager = columnStateManager ?? throw new ArgumentNullException(nameof(columnStateManager));

            _columnRegistry = columnRegistry;

            _state.CurrentPathChanged += path =>
            {
                RaisePropertyChanged(nameof(CurrentDirectory));
                PathChanged?.Invoke(path);
                
                var title = PathTitleHelper.GetTabTitle(path);

                TabTitleChanged?.Invoke(title);
            };

            _logger = loggerCreator.Create(
                category: LogCategory.UserAction,
                scope: LogScope.UserAction
                );

            directoryChangeNotifier.DirectoryChanged += OnDirectoryChanged;

            columnRegistry.PluginUnloaded += OnPluginUnloaded;

            _panelRuntime = panelContextFactory.Create();

            _navigationService = _panelRuntime.Navigation;

            _nodeSet = _panelRuntime.Nodes;

            _folderNodeContext =
                (FolderNodeContext)_nodeSet.Folder.Context;

            _fileNodeContext =
                (FileNodeContext)_nodeSet.File.Context;

            _driveNodeContext =
                (DriveNodeContext)_nodeSet.Drive.Context;

            _unifiedNodeContext =
                (UnifiedNodeContext)_nodeSet.Unified.Context;

            _workspaceController =
                _panelRuntime.WorkspaceController;

            LayoutRoot =
                _panelRuntime.LayoutController.CurrentLayout;

            _panelRuntime.LayoutController.Changed += () =>
            {
                _unifiedNodeContext.RebuildItems();

                LayoutRoot =
                    _panelRuntime.LayoutController.CurrentLayout;
            };
        }

        #endregion

        public LayoutNode _layoutRoot;

        public LayoutNode LayoutRoot
        {
            get => _layoutRoot;
            set => SetProperty(ref _layoutRoot, value);
        }

        public string GetCurrentPath() => _state.CurrentPath;

        public string GetCurrentFilePath() => _fileNodeContext.CurrentPath;

        public void SetCurrentPath(string value) => _state.CurrentPath = value;

        public IReadOnlyList<IFileItem> GetFiles() => _fileNodeContext.Files;

        public IReadOnlyList<IFolderItem> GetDirectories() => _folderNodeContext.Folders;

        public IFileNodeContext FileContext => _fileNodeContext;

        public IFolderNodeContext FolderContext => _folderNodeContext;

        public Guid GetTabToken() => _state.TabId;

        public string CurrentDirectory
        {
            get => _state.CurrentPath;
            set => _state.CurrentPath = value;
        }

        private void OnPluginUnloaded(string pluginId)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                var defsFiles = _columnRegistry.GetColumns(PanelType.Files).ToList();
                var defsFolders = _columnRegistry.GetColumns(PanelType.Folders).ToList();
                var defsDrives = _columnRegistry.GetColumns(PanelType.Drives).ToList();

                _fileNodeContext.Columns = _columnStateManager.LoadState("LeftPanel.Files", PanelType.Files, defsFiles);
                _folderNodeContext.Columns = _columnStateManager.LoadState("LeftPanel.Folders", PanelType.Folders, defsFolders);
                _driveNodeContext.Columns = _columnStateManager.LoadState("LeftPanel.Drives", PanelType.Drives, defsDrives);
            }));
        }

        public DelegateCommand<object> UpdateCommand =>
          new DelegateCommand<object>(dir =>
          {
              if (dir != null)
              {
                  var sw = Stopwatch.StartNew();

                  _navigationService.TryNavigateTo(dir.ToString(), true);

                  sw.Stop();
              }
          });

        public ITabPanelContent InitializedViewModel(
            ref Guid token,
            string path)
        {
            SetInternalCurrentPath(path);

            if (token == Guid.Empty)
                token = Guid.NewGuid();

            SetTabId(token);

            _selectionService.Register(token, _selectionManager);
            _tabStateRegistry.Register(_state);
            _navigationRegistry.Register(token, _navigationService);
            _navigationService.CurrentChanged += OnPathChanged;

            _ = SetLastPanelState();

            _workspaceController.ShowDirectoryMode(
                _nodeSet.Header,
                _nodeSet.Folder,
                _nodeSet.File);

            //_workspaceController.ShowDirectoryMode(
            //    _nodeSet.Header,
            //    _nodeSet.Unified,
            //    null);

            return this;
        }

        private void SetTabId(Guid tabId)
        {
            _state.TabId = tabId;

            _fileNodeContext.TabId = tabId;
            _folderNodeContext.TabId = tabId;
            //_driveNodeContext.TabId = tabId;
            //_navigationContext.TabId = tabId;
        }

        private void RefreshFileList(IEnumerable<FileModel> files)
        {
            using (_loggerCreator.ProfileScope(LogScope.Runtime, "RefreshFileList"))
            {
                var set = files.Select(f => f.Path).ToHashSet();

                for (int i = _fileNodeContext.Files.Count - 1; i >= 0; i--)
                {
                    if (!set.Contains(_fileNodeContext.Files[i].Path))
                        _fileNodeContext.Files.RemoveAt(i);
                }

                var existing = _fileNodeContext.Files.Select(f => f.Path).ToHashSet();

                foreach (var file in files)
                {
                    if (!existing.Contains(file.Path))
                    {
                        _fileNodeContext.Files.Add(file);
                    }
                }

                _fileNodeContext.SelectionManager.SetItems(_fileNodeContext.Files);
            }
        }

        private void RefreshDirectoryList(IEnumerable<FolderModel> dirs)
        {
            using (_loggerCreator.ProfileScope(LogScope.Runtime, "RefreshDirectoryList"))
            {
                var set = dirs.Select(d => d.Path).ToHashSet();

                for (int i = _folderNodeContext.Folders.Count - 1; i >= 0; i--)
                {
                    if (!set.Contains(_folderNodeContext.Folders[i].Path))
                        _folderNodeContext.Folders.RemoveAt(i);
                }


                var existing = _folderNodeContext.Folders.Select(d => d.Path).ToHashSet();

                foreach (var dir in dirs)
                {
                    if (!existing.Contains(dir.Path))
                    {
                        _folderNodeContext.Folders.Add(dir);
                    }
                }

                _folderNodeContext.SelectionManager.SetItems(_folderNodeContext.Folders);
            }
        }

        private async Task RefreshPanelAsync(string dirPath, CancellationToken token)
        {
            var dirsTask = _dataService.GetDirectoriesAsync(dirPath, token);
            var filesTask = _dataService.GetFilesAsync(dirPath, token);
            
            var dirs = await dirsTask;
            var files = await filesTask;
            

            if (token.IsCancellationRequested)
                return;

            RefreshDirectoryList(dirs);
            RefreshFileList(files);
        }

        #region Управление ресурсами и навигация

        private async Task SetLastPanelState()
        {
            try
            {
                if (_state.CurrentPath != VirtualPaths.MyComputer)
                {
                    var files = await _dataService.GetFilesAsync(_state.CurrentPath, CancellationToken.None);
                    var dirs = await _dataService.GetDirectoriesAsync(_state.CurrentPath, CancellationToken.None);

                    foreach (var f in files) _fileNodeContext.Files.Add(f);
                    foreach (var d in dirs) _folderNodeContext.Folders.Add(d);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Ошибка загрузки: " + ex);
            }

            var defsFiles = _columnRegistry.GetColumns(PanelType.Files).ToList();
            var defsFolders = _columnRegistry.GetColumns(PanelType.Folders).ToList();
            var defsDrives = _columnRegistry.GetColumns(PanelType.Drives).ToList();

            _fileNodeContext.Columns = _columnStateManager.LoadState("LeftPanel.Files", PanelType.Files, defsFiles);
            _folderNodeContext.Columns = _columnStateManager.LoadState("LeftPanel.Folders", PanelType.Folders, defsFolders);
            _driveNodeContext.Columns = _columnStateManager.LoadState("LeftPanel.Drives", PanelType.Drives, defsDrives);

            //await UpdateColumnValuesAsync();
        }

        private async Task GoDrivePanel()
        {
            // 1. Загружаем диски
            var drives = await _dataService.GetDrivesAsync();
            _driveNodeContext.Drives.Clear();
            foreach (var d in drives)
                _driveNodeContext.Drives.Add(d);
        }

        #endregion

        #region Обработка событий и очистка ресурсов

        private CancellationTokenSource _cts;
        
        private LoggerCreator _loggerCreator = Log.GetLoggerCreator();
       
        private void OnPathChanged(string path)
        {
            _ = OnPathChangedAsync(path);
        }

        private async Task OnPathChangedAsync(string path)
        {
            using var performance =
                _performanceProfiler.Measure(
                    "Navigation");

            var sw = Stopwatch.StartNew();

            SetInternalCurrentPath(path);

            _cts?.Cancel();
            _cts?.Dispose();

            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            if (string.IsNullOrEmpty(path) ||
                VirtualPaths.MyComputer == path)
            {
                await GoDrivePanel();

                _workspaceController.ShowMyComputerMode(
                    _nodeSet.Header,
                    _nodeSet.Drive);
            }
            else
            {
                using (_loggerCreator.ProfileScope(
                    LogScope.Runtime,
                    "Refresh Panel"))
                {
                    await RefreshPanelAsync(
                        path,
                        token);
                }

                using (_loggerCreator.ProfileScope(
                    LogScope.UI,
                    "Workspace Creation"))
                {
                    //_workspaceController.ShowDirectoryMode(
                    //    _nodeSet.Header,
                    //    _nodeSet.Unified,
                    //    null);

                    _workspaceController.ShowDirectoryMode(
                        _nodeSet.Header,
                        _nodeSet.Folder,
                        _nodeSet.File);
                }
            }
                sw.Stop();
#if (Nlog)
                _logger.Info(
                    $"Текущая папка изменена на ({GetCurrentPath()}:) " +
                    $"файлы: {_fileNodeContext.Files.Count}, " +
                    $"папки: {_folderNodeContext.Folders.Count}, " +
                    $"заняло: {sw.ElapsedMilliseconds} ms");
#endif

            performance.SetMetadata(
                "Files",
                _fileNodeContext.Files.Count);

            performance.SetMetadata(
                "Folders",
                _folderNodeContext.Folders.Count);
                
            performance.SetMetadata(
                        "Items",
                        _fileNodeContext.Files.Count +
                        _folderNodeContext.Folders.Count);

            performance.SetMetadata("Path", path);
        }

        #endregion

        #region Методы для обновления панели файлов

        private void OnDirectoryChanged(string changedPath)
        {
            if (!ShouldRefresh(changedPath, _state.CurrentPath))
                return;

            ScheduleLightRefresh(changedPath);
        }

        private void ScheduleLightRefresh(string changedPath)
        {
            if (_refreshScheduled)
                return;

            _refreshScheduled = true;

            Task.Delay(150).ContinueWith(_ =>
            {
                _refreshScheduled = false;

                Application.Current.Dispatcher.Invoke(async () =>
                {
                    await RefreshPanelAsync(_state.CurrentPath, CancellationToken.None);
                });
            });
        }

        private bool ShouldRefresh(string changedPath, string panelCurrentPath)
        {
            return changedPath.StartsWith(panelCurrentPath, StringComparison.OrdinalIgnoreCase);
        }

        public void Dispose()
        {
            _navigationService.CurrentChanged -= OnPathChanged;

            _diagnosticRegistry.Unregister(_selectionManager as IDiagnostic);
            _navigationRegistry.Remove(_state.TabId);
            _tabStateRegistry.Unregister(_state.TabId);
            //(_navigationContext as IDisposable).Dispose();
            //(_driveNodeContext as IDisposable).Dispose();
            (_folderNodeContext as IDisposable).Dispose();
            (_fileNodeContext as IDisposable).Dispose();

            _contextRegistry.TryUnregister(_fileNodeContext);
            _contextRegistry.TryUnregister(_folderNodeContext);

            base.Destroy();
        }

        private void SetInternalCurrentPath(string path)
        {
            _state.CurrentPath = path;
            _fileNodeContext.Current = _state.CurrentPath;
            _folderNodeContext.Current = _state.CurrentPath;
        }

        public void OnViewAttached(object view)
        {
            _navigationService.CurrentChanged -= OnPathChanged;
            _navigationService.CurrentChanged += OnPathChanged;
        }

        public void OnViewDetached()
        {
            _navigationService.CurrentChanged -= OnPathChanged;
        }

        #endregion

        #region Команды из Tools

        public DelegateCommand<object> GoBackDirectoryPanelCommand =>
            new DelegateCommand<object>(obj =>
            {
                if (_navigationService.CanGoBack) _navigationService.GoBack(); 
                else
                {
                    if (_state.CurrentPath != VirtualPaths.MyComputer)
                    {
                        _state.CurrentPath = VirtualPaths.MyComputer;
                        _navigationService.TryNavigateTo(VirtualPaths.MyComputer, true);
                    }
                }

#if (Nlog)
                _logger.Info($"Возврат в папку ({_state.CurrentPath})");
#endif
            });

        public DelegateCommand<object> GoDrivePanelCommand =>
            new DelegateCommand<object>(obj =>
            {
                _state.CurrentPath = VirtualPaths.MyComputer;
                _navigationService.TryNavigateTo(VirtualPaths.MyComputer, true);

                if (_state.CurrentPath == VirtualPaths.MyComputer)
                {
#if (Nlog)
                    _logger.Info($"Открыт Мой компьютер ({_state.CurrentPath})");
#endif
                    _ = GoDrivePanel();
                }
            });

        public DelegateCommand<object> UpdateDirectoryPanelCommand =>
            new DelegateCommand<object>(obj =>
            {
                _commandService.ExecuteAsync(CommandNames.Panel.Refresh);
            });

        #endregion
    }
}
