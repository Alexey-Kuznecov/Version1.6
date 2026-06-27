// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SplitPanelViewModel.cs" company="T">
//   Copyright (c) Alexei Kuznecov. All rights reserved.
// </copyright>
// <summary>
//   Реализация ViewModel для левой панели файлового менеджера. 
//   Обрабатывает навигацию, перетаскивание (drag & drop), работу с плагинами и обновление колонок.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using CommandSystem.Abstractions;
using Prism.Commands;
using Prism.Dialogs;
using Prism.Navigation.Regions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UnityCommander.Abstractions.Columns;
using UnityCommander.CommandSurface;
using UnityCommander.Common.Commands;
using UnityCommander.Common.Models.Directory;
using UnityCommander.Common.Module;
using UnityCommander.Controls.Layout;
using UnityCommander.Core;
using UnityCommander.Core.Helper;
using UnityCommander.Core.Mvvm;
using UnityCommander.Core.Navigation;
using UnityCommander.Logging.Configuration;
using UnityCommander.Logging.Contracts;
using UnityCommander.Logging.Core;
using UnityCommander.Logging.Infrastructure;
using UnityCommander.Modules.FilePanel.Columns;
using UnityCommander.Modules.FilePanel.Controllers;
using UnityCommander.Modules.FilePanel.Controllers.DnD;
using UnityCommander.Modules.FilePanel.Services;
using UnityCommander.Modules.FilePanel.States;
using UnityCommander.Services;
using UnityCommander.Services.Interfaces;
using UnityCommander.Settings;
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
        #region Поля и зависимости

        // --- Зависимости через DI
        private readonly IDataProviderService dataService;
        private readonly IMultiCommandService multiCommandService;
        private readonly NavigationManager _navigationService;
        private readonly ILogger _logger;
        private readonly ICommandUIService _commandUIService;
        private ITabRegistry _tabRegistry;
        private ISelectionManager _selectionManager;
        public bool IsActive => _tabRegistry.ActiveTab == this;
        private bool _refreshScheduled = false;
        
        private CommandExecutionService _commandService;
        private CommandPresentationProvider _presentationProvider;
        private ContextMenuController _contextMenuController;

        private readonly IColumnStateManager columnStateManager;
        private readonly IColumnRegistry columnRegistry;
        private readonly NodeContextRegistry _contextRegistry;
        private readonly TabState _state;
        public event Action<string> PathChanged;
        public event Action<string> TabTitleChanged;


        private readonly ContentNode _folderNode;
        private readonly ContentNode _fileNode;
        private readonly ContentNode _driveNode;
        private readonly ContentNode _headerNode;

        private FileNodeContext _fileNodeContext;
        private DriveNodeContext _driveNodeContext;
        private FolderNodeContext _folderNodeContext;
        private NavigationNodeContext _navigationContext;
        
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
              IMultiCommandService multiCommandService,
              IDirectoryChangeNotifier directoryChangeNotifier,
              ISelectionManager selectionManager,
              ITabRegistry tabRegistry,
              CommandSurfaceEngine surface,
              CommandPresentationProvider presentationProvider,
              IGuiCommandExecutor guiCommandExecutor,
              CommandManager manager,
              IColumnProvider columnProvider,
              IColumnStateManager columnStateManager,
              IColumnRegistry columnRegistry,
              LoggerCreator loggerCreator,
              CommandExecutionService commandService, 
              ICommandUIService commandUIService,
              ContextMenuController contextMenuController,
              GongDropAdapter dropTarget, 
              NodeContextRegistry contextRegistry, 
              ViewportMapper scrollMapper, 
              ISettingsService settingsService)
            : base(regionManager)
        {

            var setting = settingsService.Get(GeneralSettings.ShowHiddenFiles);

            _contextRegistry = contextRegistry;

            _state = new TabState();
            _state.CurrentPathChanged += path =>
            {
                RaisePropertyChanged(nameof(CurrentDirectory));
                PathChanged?.Invoke(path);
                
                var title = PathTitleHelper.GetTabTitle(path);

                TabTitleChanged?.Invoke(title);
            };

            // ЛОГЕР
            this._logger = loggerCreator.Create(
                category: LogCategory.UserAction,
                scope: LogScope.UserAction
                );

            this._contextMenuController = contextMenuController;
            this._commandService = commandService;
            this._commandUIService = commandUIService;

            this._presentationProvider = presentationProvider;
            this._selectionManager = selectionManager;
        
            this.dataService = dataService;
            this.multiCommandService = multiCommandService;
            this.multiCommandService.SaveCommand.RegisterCommand(this.SavePanelStateCommand);
            this._tabRegistry = tabRegistry ?? throw new ArgumentNullException(nameof(tabRegistry));

            this._navigationService = new NavigationManager(null);;

            directoryChangeNotifier.DirectoryChanged += OnDirectoryChanged;

            this.columnStateManager = columnStateManager ?? throw new ArgumentNullException(nameof(columnStateManager)); ;
        
            this.columnRegistry = columnRegistry;

            columnRegistry.PluginUnloaded += OnPluginUnloaded;

            var contextFactory = new NodeContextFactory(
                _navigationService, 
                _contextMenuController, 
                _selectionManager, 
                _commandUIService,
                dropTarget,
                _contextRegistry, 
                scrollMapper);

            var contentFactory = new ContentNodeFactory(contextFactory);

            _folderNode = contentFactory.CreateFolderNode();
            _fileNode = contentFactory.CreateFileNode();
            _driveNode = contentFactory.CreateDriveNode();
            _headerNode = contentFactory.CreateHeaderNode();

            _folderNodeContext = (FolderNodeContext)_folderNode.Context;
            _fileNodeContext = (FileNodeContext)_fileNode.Context;
            _driveNodeContext = (DriveNodeContext)_driveNode.Context;
            _navigationContext = (NavigationNodeContext)_headerNode.Context;

            _workspace = new Workspace(
                _headerRegion,
                _mainRegion,
                _secondaryRegion);

            _workspaceController =
                new WorkspaceController(_workspace);

            LayoutRoot = BuildLayout();
        }

        private Workspace _workspace;
        private WorkspaceController _workspaceController;

        #endregion

        public LayoutNode LayoutRoot { get; }

        public string GetCurrentPath() => _state.CurrentPath;

        public string GetCurrentFilePath() => _fileNodeContext.CurrentPath;

        public void SetCurrentPath(string value) => _state.CurrentPath = value;

        public IReadOnlyList<BaseDirectory> GetFiles() => _fileNodeContext.Files;
        
        public ISelectionManager SelectionManager => _folderNodeContext.SelectionManager;

        public Guid GetPanelToken() => _state.TabId;

        public string CurrentDirectory
        {
            get => _state.CurrentPath;
            set => _state.CurrentPath = value;
        }

        public DelegateCommand SavePanelStateCommand => new DelegateCommand(() =>
        {
            //if (settingsService.IsSessionSaved)
            //{
            //    // Логика сохранения состояния панели
            //}
        });

        private readonly RegionNode _headerRegion =
            new();

        private readonly RegionNode _mainRegion =
            new();

        private readonly RegionNode _secondaryRegion =
            new();


        private LayoutNode BuildLayout()
        {
            return new StackNode
            {
                Orientation = Orientation.Horizontal,

                Children =
                {
                    new FixedNode
                    {
                        Size = 25,
                        Content = _headerRegion
                    },

                    new SplitNode
                    {
                        Orientation = Orientation.Vertical,

                        Ratio = 0.5,

                        First = _mainRegion,

                        Second = _secondaryRegion
                    }
                }
            };
        }

        private void OnPluginUnloaded(string pluginId)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                var defsFiles = columnRegistry.GetColumns(PanelType.Files).ToList();
                var defsFolders = columnRegistry.GetColumns(PanelType.Folders).ToList();
                var defsDrives = columnRegistry.GetColumns(PanelType.Drives).ToList();

                _fileNodeContext.Columns = columnStateManager.LoadState("LeftPanel.Files", PanelType.Files, defsFiles);
                _folderNodeContext.Columns = columnStateManager.LoadState("LeftPanel.Folders", PanelType.Folders, defsFolders);
                _driveNodeContext.Columns = columnStateManager.LoadState("LeftPanel.Drives", PanelType.Drives, defsDrives);
            }));
        }

        public DelegateCommand<object> UpdateCommand =>
          new DelegateCommand<object>(dir =>
          {
              if (dir != null)
              {
#if (Nlog)
                  _logger.Info($"Текущая папка изменена на ({dir})");
#endif
                  _navigationService.TryNavigateTo(dir.ToString(), true);
              }
          });

        public ITabPanelContent InitializedViewModel(ref Guid token, string path)
        {
            SetInternalCurrentPath(path);

            if (token == Guid.Empty)
                token = Guid.NewGuid();

            _state.TabId = token;

            //NavigationContextDirectory.Instance.Register(_state.TabId, _navigationService);

            _navigationService.CurrentChanged += OnPathChanged;
            
            _ = this.SetLastPanelState();

            //_adapter = new TabContentAdapter(this);
            //_tabRegistry.Register(_adapter);

            _workspaceController.ShowDirectoryMode(_headerNode, _folderNode, _fileNode);
            return this;
        }

        private void RefreshFileList(IEnumerable<FileModel> files)
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
        }

        private void RefreshDirectoryList(IEnumerable<FolderModel> dirs)
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
        }

        private async Task RefreshPanelAsync(string dirPath, CancellationToken token)
        {
            var dirsTask = dataService.GetDirectoriesAsync(dirPath, token);
            var filesTask = dataService.GetFilesAsync(dirPath, token);

            var dirs = await dirsTask;
            var files = await filesTask;

            if (token.IsCancellationRequested)
                return; // ❌ устарело — убиваем

            RefreshDirectoryList(dirs);
            RefreshFileList(files);

            //await UpdateColumnValuesAsync();
        }

        #region Управление ресурсами и навигация

        private async Task SetLastPanelState()
        {
            try
            {
                if (_state.CurrentPath != VirtualPaths.MyComputer)
                {
                    var sw = Stopwatch.StartNew();

                    var files = await dataService.GetFilesAsync(_state.CurrentPath, CancellationToken.None);
                    var dirs = await dataService.GetDirectoriesAsync(_state.CurrentPath, CancellationToken.None);

                    foreach (var f in files) _fileNodeContext.Files.Add(f);
                    foreach (var d in dirs) _folderNodeContext.Folders.Add(d);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Ошибка загрузки: " + ex);
            }

            var defsFiles = columnRegistry.GetColumns(PanelType.Files).ToList();
            var defsFolders = columnRegistry.GetColumns(PanelType.Folders).ToList();
            var defsDrives = columnRegistry.GetColumns(PanelType.Drives).ToList();

            _fileNodeContext.Columns = columnStateManager.LoadState("LeftPanel.Files", PanelType.Files, defsFiles);
            _folderNodeContext.Columns = columnStateManager.LoadState("LeftPanel.Folders", PanelType.Folders, defsFolders);
            _driveNodeContext.Columns = columnStateManager.LoadState("LeftPanel.Drives", PanelType.Drives, defsDrives);

            //await UpdateColumnValuesAsync();
        }

        private async Task GoDrivePanel()
        {
            // 1. Загружаем диски
            var drives = await dataService.GetDrivesAsync();
            _driveNodeContext.Drives.Clear();
            foreach (var d in drives)
                _driveNodeContext.Drives.Add(d);
        }

        #endregion

        #region Обработка событий и очистка ресурсов

        private CancellationTokenSource _cts;

        private void OnPathChanged(string path)
        {
            SetInternalCurrentPath(path);

            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            if (string.IsNullOrEmpty(path) || VirtualPaths.MyComputer == path)
            {
                _ = this.GoDrivePanel();
                _workspaceController.ShowMyComputerMode(_headerNode, _driveNode);
            }
            else
            {
                _ = RefreshPanelAsync(path, _cts.Token);
                _workspaceController.ShowDirectoryMode(_headerNode, _folderNode, _fileNode);
            }
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
            this.multiCommandService.SaveCommand.UnregisterCommand(this.SavePanelStateCommand);

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
