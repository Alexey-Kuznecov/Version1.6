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
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using UnityCommander.CommandSurface;
using UnityCommander.Common.Commands;
using UnityCommander.Common.Models.Directory;
using UnityCommander.Common.Module;
using UnityCommander.Core;
using UnityCommander.Core.DragDrop;
using UnityCommander.Core.Helper;
using UnityCommander.Core.Mvvm;
using UnityCommander.Core.Navigation;
using UnityCommander.Integration.Plugins;
using UnityCommander.Logging.Configuration;
using UnityCommander.Logging.Contracts;
using UnityCommander.Logging.Core;
using UnityCommander.Logging.Infrastructure;
using UnityCommander.Modules.FilePanel.Columns;
using UnityCommander.Modules.FilePanel.Controllers;
using UnityCommander.Modules.FilePanel.Layout;
using UnityCommander.Modules.FilePanel.Services;
using UnityCommander.Modules.FilePanel.States;
using UnityCommander.Services;
using UnityCommander.Services.Interfaces;
using UnityCommander.Services.Interfaces.Settings;

namespace UnityCommander.Modules.FilePanel.ViewModels
{
    /// <summary>
    /// Представляет ViewModel левой панели файлового менеджера.
    /// Реализует обработку навигации, команд, drag & drop и интеграцию плагинных колонок.
    /// </summary>
    [Serializable]
    public class SplitPanelViewModel : RegionViewModelBase, IDropTarget, IDirectoryPanel
    {
        #region Поля и зависимости

        // --- Зависимости через DI
        private readonly IDialogService dialogService;
        private readonly IDataProviderService dataService;
        private readonly ISettings settingsService;
        private readonly IMultiCommandService multiCommandService;
        //private readonly IAppLogger _appLogger;
        private readonly NavigationManager _navigationService;
        private readonly CommandManager commandManager;
        private readonly ILogger _logger;
        private readonly ICommandUIService _commandUIService;
        //private readonly TabState _state;
        private ITabRegistry _tabRegistry;
        private TabContentAdapter _adapter;
        private ISelectionManager _selectionManager;
        public bool IsActive => _tabRegistry.ActiveTab == this;

        // Поля из дополнительной части (Tools)
        private bool _refreshScheduled = false;
        private CommandService _commandService;
        private CommandPresentationProvider _presentationProvider;
        private ContextMenuController _contextMenuController;

        private readonly IColumnStateManager columnStateManager;
        private readonly ColumnRegistry columnRegistry;
        private readonly ColumnController<FileModel> _fileColumnController;
        private readonly ColumnController<FolderModel> _folderColumnController;
        private readonly ISettingsStore settings;
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
              ISettingsProviderService settingsService,
              IDataProviderService dataService,
              IMultiCommandService multiCommandService,
              IPluginLoaderService pluginService,
              IGlobalCommandService globalCommandService,
              IIconProviderService iconProvider,
              IAppConfigService configService,
              IDirectoryChangeNotifier directoryChangeNotifier,
              ISelectionManager selectionManager,
              ITabRegistry tabRegistry,
              CommandSurfaceEngine surface,
              CommandPresentationProvider presentationProvider,
              IGuiCommandExecutor guiCommandExecutor,
              CommandManager manager,
              IColumnProvider columnProvider,
              IColumnStateManager columnStateManager,
              ISettingsStore settingsStore,
              ColumnRegistry columnRegistry,
              LoggerCreator loggerCreator,
              CommandService commandService, 
              ICommandUIService commandUIService,
              ContextMenuController contextMenuController)
            : base(regionManager)
        {
            this._logger = loggerCreator.Create(
                category: LogCategory.UserAction,
                scope: LogScope.UserAction
                );

            this._contextMenuController = contextMenuController;
            this._commandService = commandService;
            this._commandUIService = commandUIService;

            this._presentationProvider = presentationProvider;
            this._selectionManager = selectionManager;
            this.dialogService = dialogService;
            this.commandManager = manager;
        

            this.dataService = dataService;
            this.settingsService = settingsService.GetAppConfig();
            this.multiCommandService = multiCommandService;
            this.multiCommandService.SaveCommand.RegisterCommand(this.SavePanelStateCommand);
            this._tabRegistry = tabRegistry ?? throw new ArgumentNullException(nameof(tabRegistry));

            this._navigationService = new NavigationManager(null);;

            directoryChangeNotifier.DirectoryChanged += OnDirectoryChanged;

            this.columnStateManager = columnStateManager ?? throw new ArgumentNullException(nameof(columnStateManager)); ;
            this.settings = settingsStore ?? throw new ArgumentNullException(nameof(settingsStore));
            this.columnRegistry = columnRegistry ?? throw new ArgumentNullException(nameof(settingsStore));

            //_fileColumnController = new ColumnController<FileModel>(_state, columnRegistry, columnStateManager);
            //_folderColumnController = new ColumnController<FolderModel>(_state, columnRegistry, columnStateManager);

            var contextFactory = new NodeContextFactory(
                _navigationService, 
                _contextMenuController, 
                _selectionManager, 
                _commandUIService);

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

        public string GetCurrentPath() => _folderNodeContext.Current;

        public string GetCurrentFilePath() => _fileNodeContext.CurrentPath;

        public void SetCurrentPath(string value) => _folderNodeContext.Current = value;

        public IReadOnlyList<BaseDirectory> GetFiles() => _fileNodeContext.Files;
        
        public ISelectionManager SelectionManager => _folderNodeContext.SelectionManager;
        
        public Guid Token { get; set; }

        public Guid GetPanelToken() => Token;

        public string CurrentDirectory
        {
            get => _folderNodeContext.Current;
            set
            {
                if (_folderNodeContext.Current == value)
                    return;

                _folderNodeContext.Current = value;

                RaisePropertyChanged();

                PathChanged?.Invoke(value);

                var title = PathTitleHelper.GetTabTitle(value);

                TabTitleChanged?.Invoke(title);
            }
        }

        public DelegateCommand SavePanelStateCommand => new DelegateCommand(() =>
        {
            if (settingsService.IsSessionSaved)
            {
                // Логика сохранения состояния панели
            }
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

        #region Обработка Drag-and-Drop

        /// <summary>
        /// Обрабатывает событие DragOver, устанавливая визуальные эффекты для корректного отображения adorner.
        /// </summary>
        /// <param name="dropInfo">Информация о событии перетаскивания.</param>
        void IDropTarget.DragOver(IDropInfo dropInfo)
        {
            // Проверяем, есть ли реально выбранные элементы
            bool hasElements = false;

            if (dropInfo.Data is BaseDirectory)
                hasElements = true;
            else if (dropInfo.Data is IList list && list.Count > 0)
                hasElements = true;

            // Если драг начат с пустого места (нет элементов) — блокируем драг
            if (!hasElements)
            {
                dropInfo.Effects = DragDropEffects.None;
                dropInfo.DropTargetAdorner = null;
                return;
            }

            // Если драг идёт по элементу — разрешаем
            bool isMultiSelect = dropInfo.Data is List<object> && dropInfo.TargetItem is ListBox or BaseDirectory;
            bool isSingleSelect = dropInfo.Data is BaseDirectory && dropInfo.TargetItem is ListBox or BaseDirectory;

            var adorner = AdornerLayer.GetAdornerLayer(dropInfo.VisualTarget);
            if (adorner == null)
                this.CreateAdornerLayer(dropInfo.VisualTarget);

            if (isMultiSelect || isSingleSelect)
                dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;

            dropInfo.Effects = DragDropEffects.Copy;
        }

        /// <summary>
        /// Обрабатывает событие Drop, инициируя диалог копирования и передачу параметров.
        /// </summary>
        /// <param name="dropInfo">Информация о событии Drop.</param>
        void IDropTarget.Drop(IDropInfo dropInfo)
        {
            var visualTarget = dropInfo.VisualTarget as ListBox;
            var splitPanelViewModel = visualTarget?.DataContext as SplitPanelViewModel;

            string targetPath = null;
            var targetItem = dropInfo.TargetItem as BaseDirectory;

            // Определяем путь назначения
            if (targetItem == null)
            {
                var firstItem = visualTarget?.SelectedItem as BaseDirectory;
                if (firstItem != null)
                {
                    var pathParts = firstItem.Path.Split('\\');
                    targetPath = System.IO.Path.Combine(pathParts.Take(pathParts.Length - 1).ToArray());
                }
                else
                {
                    targetPath = splitPanelViewModel?.CurrentDirectory ?? this.CurrentDirectory;
                }
            }
            else
            {
                targetPath = targetItem.Path;
            }

            // Собираем список исходных элементов
            List<string> sourcePaths = new();
            if (dropInfo.Data is BaseDirectory single)
            {
                sourcePaths.Add(single.Path);
            }
            else if (dropInfo.Data is IList list)
            {
                foreach (var item in list)
                {
                    if (item is BaseDirectory dir)
                        sourcePaths.Add(dir.Path);
                }
            }

            // Отправляем **в одно окно** все исходные пути
            this.dialogService.ShowDialog("CopyDialog",
                new OverrideDialogParameters(new CopyParameters
                {
                    ManySource = sourcePaths,
                    Target = targetPath
                }), r => { });
        }

        /// <summary>
        /// Создаёт слой adorner для указанного элемента, если он отсутствует.
        /// </summary>
        /// <param name="element">UI-элемент, для которого создаётся adorner.</param>
        private void CreateAdornerLayer(UIElement element)
        {
            if (element is ListBox listBox && listBox.Parent is Grid parent)
            {
                parent.Children.Remove(listBox);
                var decorator = new AdornerDecorator { Child = listBox };
                parent.Children.Add(decorator);
            }
        }

        #endregion

        public ITabPanelContent InitializedViewModel(ref Guid token, string path)
        {
            this.CurrentDirectory = path;
            _folderNodeContext.Current = path;
            if (token == Guid.Empty)
                token = Guid.NewGuid();

            Token = token;

            NavigationContextDirectory.Instance.Register(Token, _navigationService);

            _navigationService.CurrentChanged += OnPathChanged;
            
            _ = this.SetLastPanelState();

            _adapter = new TabContentAdapter(this);
            _tabRegistry.Register(_adapter);


            _workspaceController.ShowDirectoryMode(_headerNode, _folderNode, _fileNode);
            return this;
        }

        #region Новая система колонок

        private async Task UpdateColumnValuesAsync()
        {
            var fileColumns = columnRegistry.GetColumns(PanelType.Files).ToList();
            var folderColumns = columnRegistry.GetColumns(PanelType.Folders).ToList();

            var folderUpdates = new List<(FolderModel folder, string columnId, object value)>();
            var fileUpdates = new List<(FileModel file, string columnId, object value)>();

            await Task.Run(() =>
            {
                foreach (var folder in _folderNodeContext.Folders)
                    foreach (var column in folderColumns)
                        folderUpdates.Add((folder, column.Id, column.ColumnValueHandler(folder)));

                foreach (var file in _fileNodeContext.Files)
                    foreach (var column in fileColumns)
                        fileUpdates.Add((file, column.Id, column.ColumnValueHandler(file)));
            });

            Application.Current.Dispatcher.Invoke(() =>
            {
                foreach (var u in folderUpdates)
                    u.folder.Additional[u.columnId] = u.value;
                foreach (var u in fileUpdates)
                    u.file.Additional[u.columnId] = u.value;
            });
        }

        private void RefreshFileList(IEnumerable<FileModel> files)
        {
            if (_fileNodeContext.Files == null)
                _fileNodeContext.Files.Clear();

            var newFiles = files.ToDictionary(f => f.Path, f => f);

            for (int i = _fileNodeContext.Files.Count - 1; i >= 0; i--)
            {
                if (!newFiles.ContainsKey(_fileNodeContext.Files[i].Path))
                    _fileNodeContext.Files.RemoveAt(i);
            }

            foreach (var file in files)
            {
                if (!_fileNodeContext.Files.Any(f => f.Path == file.Path))
                    _fileNodeContext.Files.Add(file);
            }
        }

        private void RefreshDirectoryList(IEnumerable<FolderModel> dirs)
        {
            if (_folderNodeContext.Folders == null)
                _folderNodeContext.Folders.Clear();

            var newDirs = dirs.ToDictionary(d => d.Path, d => d);

            for (int i = _folderNodeContext.Folders.Count - 1; i >= 0; i--)
            {
                if (!newDirs.ContainsKey(_folderNodeContext.Folders[i].Path))
                    _folderNodeContext.Folders.RemoveAt(i);
            }

            foreach (var dir in dirs)
            {
                if (!_folderNodeContext.Folders.Any(f => f.Path == dir.Path))
                    _folderNodeContext.Folders.Add(dir);
            }
        }

        private async Task RefreshPanelAsync(string dirPath)
        {
            var sw = Stopwatch.StartNew();

            var dirsTask = dataService.GetDirectoriesAsync(dirPath);
            sw.Stop();
            Debug.WriteLine($"Dirs loaded: {sw.ElapsedMilliseconds}");
            sw.Restart();
            var filesTask = dataService.GetFilesAsync(dirPath);
            sw.Stop();
            Debug.WriteLine($"Files loaded: {sw.ElapsedMilliseconds}");
            var dirs = await dirsTask;
            var files = await filesTask;
            sw.Restart();
            RefreshDirectoryList(dirs);
            sw.Stop();
            Debug.WriteLine($"Apply dirs: {sw.ElapsedMilliseconds}");
            sw.Restart();
            RefreshFileList(files);

            sw.Stop();
            Debug.WriteLine($"Apply files: {sw.ElapsedMilliseconds}");
            this.CurrentDirectory = dirPath;
            await UpdateColumnValuesAsync();
            //Debug.WriteLine($"Columns updated: {sw.ElapsedMilliseconds}");
            //sw.Stop();
        }

        #endregion

        #region Управление ресурсами и навигация

        private async Task SetLastPanelState()
        {
            try
            {
                if (this.CurrentDirectory != VirtualPaths.MyComputer)
                {
                    var sw = Stopwatch.StartNew();

                    var files = await dataService.GetFilesAsync(this.CurrentDirectory);
                    this._logger.Debug($"Files: {sw.Elapsed}");

                    var dirs = await dataService.GetDirectoriesAsync(this.CurrentDirectory);
                    this._logger.Debug($"Dirs: {sw.Elapsed}");

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

            await UpdateColumnValuesAsync();
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

        private void OnPathChanged(string path)
        {
            var sw = Stopwatch.StartNew();
            _navigationContext.CurrentPath = path;

          
            if (string.IsNullOrEmpty(path) || VirtualPaths.MyComputer == path)
            {
                _ = this.GoDrivePanel();
                _workspaceController.ShowMyComputerMode(_headerNode, _driveNode);
            }
            else
            {
                _ = RefreshPanelAsync(path);
                _workspaceController.ShowDirectoryMode(_headerNode, _folderNode, _fileNode);
            }

            sw.Stop();

            Debug.WriteLine($"OnPathChanged: {sw.ElapsedMilliseconds} ms");
        }

        public override void Destroy()
        {
            _navigationService.CurrentChanged -= OnPathChanged;
            //columnSync.ColumnChanged -= OnColumnChanged;
            _tabRegistry.Unregister(_adapter.TabId);
            this.multiCommandService.SaveCommand.UnregisterCommand(this.SavePanelStateCommand);
            base.Destroy();
        }

        #endregion

        #region Методы для обновления панели файлов

        private void OnDirectoryChanged(string changedPath)
        {
            if (!ShouldRefresh(changedPath, this.CurrentDirectory))
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
                    await RefreshPanelAsync(CurrentDirectory);
                });
            });
        }

        private bool ShouldRefresh(string changedPath, string panelCurrentPath)
        {
            return changedPath.StartsWith(panelCurrentPath, StringComparison.OrdinalIgnoreCase);
        }

        #endregion

        #region Команды из Tools

        public DelegateCommand<object> GoBackDirectoryPanelCommand =>
            new DelegateCommand<object>(obj =>
            {
                if (_navigationService.CanGoBack) _navigationService.GoBack(); 
                else
                {
                    if (this.CurrentDirectory != VirtualPaths.MyComputer)
                    {
                        this.CurrentDirectory = VirtualPaths.MyComputer;
                        _navigationService.TryNavigateTo(VirtualPaths.MyComputer, true);
                    }
                }

#if (Nlog)
                _logger.Info($"Возврат в папку ({this.CurrentDirectory})");
#endif
            });

        public DelegateCommand<object> GoDrivePanelCommand =>
            new DelegateCommand<object>(obj =>
            {
                this.CurrentDirectory = VirtualPaths.MyComputer;
                _navigationService.TryNavigateTo(VirtualPaths.MyComputer, true);

                if (this.CurrentDirectory == VirtualPaths.MyComputer)
                {
#if (Nlog)
                    _logger.Info($"Открыт Мой компьютер ({this.CurrentDirectory})");
#endif
                    _ = GoDrivePanel();
                }
            });

        public DelegateCommand<object> UpdateDirectoryPanelCommand =>
            new DelegateCommand<object>(obj =>
            {
                _commandService.Execute(CommandNames.Panel.Refresh);
            });

        #endregion
    }
}
