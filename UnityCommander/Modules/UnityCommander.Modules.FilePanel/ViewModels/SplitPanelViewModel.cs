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
using System.Collections.ObjectModel;
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
using UnityCommander.Common.Models.Icons;
using UnityCommander.Common.Module;
using UnityCommander.Common.Selection;
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
        private readonly IGlobalCommandService globalCommandService;
        private readonly IPluginLoaderService pluginLoaderService;
        private readonly IAppConfigService configService;
        //private readonly IAppLogger _appLogger;
        private readonly NavigationManager _navigationService;
        private readonly CommandManager commandManager;
        private readonly ILogger _logger;
        private readonly ICommandUIService _commandUIService;
        private readonly TabState _state;
        private ITabRegistry _tabRegistry;
        private TabContentAdapter _adapter;
        private ISelectionManager _selectionManager;
        private ISelectionContext _selectionContext;
        public bool IsActive => _tabRegistry.ActiveTab == this;

        // --- Прочие поля
        private BaseDirectory selectedCurrentDirectoryItem;
        private ObservableCollection<FileModel> fileList;
        private ObservableCollection<FolderModel> directoryList;
        private ObservableCollection<DriveModel> driveList;
        private ObservableCollection<MenuItemViewModel> _contextMenuItems = new();
        private ControlTemplate directoryPanelTemplate;
        private object selectedDirectory;
        private FileModel currentFile;

        // Поля из дополнительной части (Tools)
        private IIcon thisComputerIcon;
        private IIcon backButtonIcon;
        private IIcon updateDirectoryPanelIcon;
        private bool thisComputerIconIsEnabled;
        private bool backButtonIsEnabled;
        private bool _refreshScheduled = false;
        private IEnumerable<ColumnModel> fileViewColumns;
        private IEnumerable<ColumnModel> folderViewColumns;
        private IEnumerable<ColumnModel> driveViewColumns;
        private IColumnProvider columnProvider;
        private CommandService _commandService;
        private CommandPresentationProvider _presentationProvider;
        private ContextMenuController _contextMenuController;
        private readonly IColumnStateManager columnStateManager;
        private readonly ColumnRegistry columnRegistry;
        private readonly ISettingsStore settings;
        public event Action<string> PathChanged;
        public event Action<string> TabTitleChanged;
        private readonly CommandSurfaceEngine _surface;
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
            this._commandUIService = commandUIService;
            this._state = new();
            this._surface = surface;
            this._commandService = commandService;
            this._presentationProvider = presentationProvider;
            this._selectionManager = selectionManager;
            this.configService = configService;
            this.dialogService = dialogService;
            this.commandManager = manager;
        
            this.pluginLoaderService = pluginService;
            this.dataService = dataService;
            this.settingsService = settingsService.GetAppConfig();
            this.globalCommandService = globalCommandService;
            this.multiCommandService = multiCommandService;
            this.multiCommandService.SaveCommand.RegisterCommand(this.SavePanelStateCommand);
            this._tabRegistry = tabRegistry ?? throw new ArgumentNullException(nameof(tabRegistry));

            // Инициализация иконок

            var commandMyComputer 
                = this._commandUIService.Create(CommandNames.Navigation.Drives);
            var commandBack 
                = this._commandUIService.Create(CommandNames.Navigation.Goto);
            var commandForward 
                = this._commandUIService.Create(CommandNames.Navigation.Goto);
            var commandRefresh 
                = this._commandUIService.Create(CommandNames.Navigation.Refresh);

            this.ThisComputerIcon = iconProvider.GetIcon(MaterialDesignThemes.Wpf.PackIconKind.LaptopWindows);
            this.BackButtonIcon = iconProvider.GetIcon(MaterialDesignThemes.Wpf.PackIconKind.ArrowBack);
            this.UpdateDirectoryIcon = iconProvider.GetIcon(MaterialDesignThemes.Wpf.PackIconKind.Refresh);
            this.ThisComputerIconIsEnabled = true;
            this.BackButtonIsEnabled = true;
            this._navigationService = new NavigationManager(null);
            this.DriveList.Clear();

            directoryChangeNotifier.DirectoryChanged += OnDirectoryChanged;
            // заменить локальные new/недостающие поля на внедрённые
            this.columnProvider = columnProvider ?? throw new ArgumentNullException(nameof(columnProvider));
            this.columnStateManager = columnStateManager ?? throw new ArgumentNullException(nameof(columnStateManager)); ;
            this.settings = settingsStore ?? throw new ArgumentNullException(nameof(settingsStore));
            this.columnRegistry = columnRegistry ?? throw new ArgumentNullException(nameof(settingsStore));
            // подпись на главный обработчик (одно место)
            ColumnSyncService.RegisterHandler("Main", width => OnRemoteWidthChanged("Main", width));
            
            LayoutRoot = BuildLayout();
        }

        #endregion

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
                        Content = new ContentNode { Key = ContentViewType.Header }
                    },

                    //new ContentNode { Key = "NavigationPanel" },

                    new SplitNode
                    {
                        Orientation = Orientation.Vertical,
                        Ratio = 0.5,

                        First = new ContentNode { Key = ContentViewType.Directory },
                        Second = new ContentNode { Key = ContentViewType.File }
                    }
                }
            };
        }

        public LayoutNode LayoutRoot { get; }

        #region Синхронизация колонок
        
        public void UpdateColumnWidth(ColumnModel column, double newWidth)
        {
            if (column == null) return;
            column.Width = newWidth;
            if (!string.IsNullOrEmpty(column.SyncGroup))
                ColumnSyncService.NotifyWidthChanged(column.SyncGroup, newWidth); // ← double, а не column
        }

        private void OnRemoteWidthChanged(string v, double width)
        {
            throw new NotImplementedException();
        }
        
        #endregion

        #region Колонки

        public IEnumerable<ColumnModel> FileViewColumns
        {
            get => this.fileViewColumns;
            set
            {
                this.SetProperty(ref this.fileViewColumns, value);
            }
        }

        public IEnumerable<ColumnModel> FolderViewColumns
        {
            get => this.folderViewColumns;
            set
            {
                this.SetProperty(ref this.folderViewColumns, value);
            }
        }

        public IEnumerable<ColumnModel> DriveViewColumns
        {
            get => this.driveViewColumns;
            set
            {
                this.SetProperty(ref this.driveViewColumns, value);
            }
        }

        #endregion

        #region Реализация интерфейсов IDirectoryPanel

        public Guid Token { get; set; }

        public Guid GetPanelToken() => this.Token;

        public string GetCurrentPath() => this.CurrentDirectory;

        public string GetCurrentFilePath() => this.CurrentFile?.Path;

        public void SetCurrentPath(string value) => this.CurrentDirectory = value;

        public IReadOnlyList<BaseDirectory> GetFiles() => this.FileList;
        #endregion

        #region Свойства (Properties)

        public ISelectionManager SelectionManager => this._selectionManager;

        //public ObservableCollection<BaseDirectory> Items { get; set; } = new();
        public ObservableCollection<BaseDirectory> SelectedItems
            => _state.SelectedItems;

        public string CurrentDirectory
        {
            get => _state.CurrentDirectory;
            set
            {
                if (_state.CurrentDirectory == value)
                    return;

                _state.CurrentDirectory = value;

                RaisePropertyChanged();

                PathChanged?.Invoke(value);

                var title = PathTitleHelper.GetTabTitle(value);

                TabTitleChanged?.Invoke(title);
            }
        }

        public object SelectedDirectory
        {
            get => _state.SelectedDirectory;
            set
            {
                if (_state.SelectedDirectory == value)
                    return;

                _state.SelectedDirectory = value;
                
                RaisePropertyChanged();
            }
        }

        public FileModel CurrentFile
        {
            get => _state.CurrentFile;
            set
            {
                if (_state.CurrentFile == value)
                    return;

                _state.CurrentFile = value;

                RaisePropertyChanged();
            }
        }
        public ObservableCollection<MenuItemViewModel> ContextMenuItems
            => _state.ContextMenuItems;

        public ObservableCollection<FolderModel> DirectoryList
            => _state.Directories;

        public ObservableCollection<FileModel> FileList
            => _state.Files;

        public ObservableCollection<DriveModel> DriveList
            => _state.Drives;

        public ControlTemplate DirectoryPanelTemplate
        {
            get => this.directoryPanelTemplate;
            set => this.SetProperty(ref this.directoryPanelTemplate, value);
        }

        /// <summary>
        /// Команда для выбора текущего элемента директории.
        /// </summary>
        public DelegateCommand<BaseDirectory> SelectCurrentDirectoryItem =>
            new DelegateCommand<BaseDirectory>(item => this.SelectedCurrentDirectoryItem = item);


        public BaseDirectory SelectedCurrentDirectoryItem
        {
            get => this.selectedCurrentDirectoryItem;
            set => this.SetProperty(ref this.selectedCurrentDirectoryItem, value);
        }

        #endregion

        #region Свойства из Tools

        public IIcon ThisComputerIcon
        {
            get => this.thisComputerIcon;
            set => this.SetProperty(ref this.thisComputerIcon, value);
        }

        public IIcon BackButtonIcon
        {
            get => this.backButtonIcon;
            set => this.SetProperty(ref this.backButtonIcon, value);
        }

        public IIcon UpdateDirectoryIcon
        {
            get => this.updateDirectoryPanelIcon;
            set => this.SetProperty(ref this.updateDirectoryPanelIcon, value);
        }

        public bool ThisComputerIconIsEnabled
        {
            get => this.thisComputerIconIsEnabled;
            set => this.SetProperty(ref this.thisComputerIconIsEnabled, value);
        }

        public bool BackButtonIsEnabled
        {
            get => this.backButtonIsEnabled;
            set => this.SetProperty(ref this.backButtonIsEnabled, value);
        }

        #endregion

        #region Команды

        public DelegateCommand<FolderModel> NavigateDirectoryCommand =>
            new DelegateCommand<FolderModel>(dir =>
            {
                if (dir != null)
                {
#if (Nlog)
                    _logger.Info($"Открыта папка ({dir.Path})");
#endif
                    _navigationService.TryNavigateTo(dir.Path);
                }
            });

        public DelegateCommand<DriveModel> GotoDiskCommand =>
            new DelegateCommand<DriveModel>(dir =>
            {
                if (dir != null)
                {
#if (Nlog)
                    _logger.Info($"Открыт диск ({dir.Letter})");
#endif
                    _navigationService.TryNavigateTo(dir.Letter);
                }
            });

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

        public DelegateCommand SavePanelStateCommand => new DelegateCommand(() =>
        {
            if (settingsService.IsSessionSaved)
            {
                // Логика сохранения состояния панели
            }
        });

        public DelegateCommand<object> ShowContextMenuCommand =>
             new DelegateCommand<object>(parameter =>
             {
                 _contextMenuController.Show(_state, parameter);
             });

        #endregion

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

        #region Инициализация панели

        public ITabPanelContent InitializedViewModel(ref Guid token, string path)
        {
            this.CurrentDirectory = path;

            if (token == Guid.Empty)
                token = Guid.NewGuid();

            this.Token = token;

            NavigationContextDirectory.Instance.Register(this.Token, _navigationService);

            _navigationService.CurrentChanged += OnPathChanged;
            
            _ = this.SetLastPanelState();

            _adapter = new TabContentAdapter(this);
            _tabRegistry.Register(_adapter);

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
                foreach (var folder in DirectoryList)
                    foreach (var column in folderColumns)
                        folderUpdates.Add((folder, column.Id, column.ColumnValueHandler(folder)));

                foreach (var file in FileList)
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
            if (FileList == null)
                FileList.Clear();

            var newFiles = files.ToDictionary(f => f.Path, f => f);

            for (int i = FileList.Count - 1; i >= 0; i--)
            {
                if (!newFiles.ContainsKey(FileList[i].Path))
                    FileList.RemoveAt(i);
            }

            foreach (var file in files)
            {
                if (!FileList.Any(f => f.Path == file.Path))
                    FileList.Add(file);
            }
        }

        private void RefreshDirectoryList(IEnumerable<FolderModel> dirs)
        {
            if (DirectoryList == null)
                DirectoryList.Clear();

            var newDirs = dirs.ToDictionary(d => d.Path, d => d);

            for (int i = DirectoryList.Count - 1; i >= 0; i--)
            {
                if (!newDirs.ContainsKey(DirectoryList[i].Path))
                    DirectoryList.RemoveAt(i);
            }

            foreach (var dir in dirs)
            {
                if (!DirectoryList.Any(f => f.Path == dir.Path))
                    DirectoryList.Add(dir);
            }
        }

        private async Task RefreshPanelAsync(string dirPath)
        {
            var dirsTask = dataService.GetDirectoriesAsync(dirPath);
            var filesTask = dataService.GetFilesAsync(dirPath);

            var dirs = await dirsTask;
            var files = await filesTask;

            RefreshDirectoryList(dirs);
            RefreshFileList(files);

            this.CurrentDirectory = dirPath;
            await UpdateColumnValuesAsync();
        }

        #endregion

        #endregion

        #region Управление ресурсами и навигация

        private async Task SetLastPanelState()
        {
            // Загружаем пустые коллекции
            //FileList.Clear();
            //DirectoryList.Clear();

            try
            {
                if (this.CurrentDirectory != VirtualPaths.MyComputer)
                {
                    this.DirectoryPanelTemplate = (ControlTemplate)Application.Current.FindResource("DirectoryListViewTemplate");
                    var files = await dataService.GetFilesAsync(this.CurrentDirectory);
                    var dirs = await dataService.GetDirectoriesAsync(this.CurrentDirectory);
                    foreach (var f in files) FileList.Add(f);
                    foreach (var d in dirs) DirectoryList.Add(d);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Ошибка загрузки: " + ex);
            }

            var defsFiles = columnRegistry.GetColumns(PanelType.Files).ToList();
            var defsFolders = columnRegistry.GetColumns(PanelType.Folders).ToList();
            var defsDrives = columnRegistry.GetColumns(PanelType.Drives).ToList();

            FileViewColumns = columnStateManager.LoadState("LeftPanel.Files", PanelType.Files, defsFiles);
            FolderViewColumns = columnStateManager.LoadState("LeftPanel.Folders", PanelType.Folders, defsFolders);
            DriveViewColumns = columnStateManager.LoadState("LeftPanel.Drives", PanelType.Drives, defsDrives);

            await UpdateColumnValuesAsync();
        }

        private async Task GoDrivePanel()
        {
            // 1. Загружаем диски
            var drives = await dataService.GetDrivesAsync();
            DriveList.Clear();
            foreach (var d in drives)
                DriveList.Add(d);

            // 3. Другое состояние UI
            ThisComputerIconIsEnabled = false;
            BackButtonIsEnabled = true;
        }

        #endregion

        #region Обработка событий и очистка ресурсов

        private void OnPathChanged(string path)
        {
            if (string.IsNullOrEmpty(path) || VirtualPaths.MyComputer == path)
            {
                DirectoryPanelTemplate = (ControlTemplate)Application.Current.FindResource("DriveListViewTemplate");
                _ = this.GoDrivePanel();
                
                this.ThisComputerIconIsEnabled = false;
            }
            else
            {
                this.DirectoryPanelTemplate = (ControlTemplate)Application.Current.FindResource("DirectoryListViewTemplate");
                _ = RefreshPanelAsync(path);
                this.ThisComputerIconIsEnabled = true;
                this.BackButtonIsEnabled = true;
            }
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
                        _navigationService.TryNavigateTo(VirtualPaths.MyComputer, true); // root
                    }

                    this.BackButtonIsEnabled = false;
                }

#if (Nlog)
                _logger.Info($"Возврат в папку ({this.CurrentDirectory})");
#endif
            });

        public DelegateCommand<object> GoDrivePanelCommand =>
            new DelegateCommand<object>(obj =>
            {
                this.CurrentDirectory = VirtualPaths.MyComputer;
                _navigationService.TryNavigateTo(VirtualPaths.MyComputer, true); // root
                this.ThisComputerIconIsEnabled = false;

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

        public void NavigateTo(string? path)
        {
            if (_navigationService.IsValidPath(path))
                _navigationService.TryNavigateTo(path);
        }

        #endregion
    }
}
