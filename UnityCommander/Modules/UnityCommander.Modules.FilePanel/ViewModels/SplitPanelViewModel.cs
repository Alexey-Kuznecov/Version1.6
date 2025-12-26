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
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using UnityCommander.Common;
using UnityCommander.Common.Models.Directory;
using UnityCommander.Common.Models.Icons;
using UnityCommander.Common.Module;
using UnityCommander.Common.Selection;
using UnityCommander.Core;
using UnityCommander.Core.DragDrop;
using UnityCommander.Core.Mvvm;
using UnityCommander.Core.Navgator;
using UnityCommander.Integration.Columns;
using UnityCommander.Integration.Commands;
using UnityCommander.Integration.Enums;
using UnityCommander.Integration.Plugins;
using UnityCommander.Logging.Abstractions;
using UnityCommander.Modules.FilePanel.Columns;
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
        private IGuiCommandExecutor guiCommandExecutor;
        private IPanelRegistry _panelRegistry;
        private PanelViewModelAdapter _adapter;
        private ISelectionManager _selectionManager;  
        private ISelectionContext _selectionContext;
        public bool IsActive => _panelRegistry.ActivePanel == this;

        // --- Прочие поля
        public ObservableCollection<BaseDirectory> Items { get; set; } = new();
        public ObservableCollection<BaseDirectory> SelectedItems { get; set; } = new();
        private BaseDirectory selectedCurrentDirectoryItem;
        private ObservableCollection<FileModel> fileList;
        private ObservableCollection<FolderModel> directoryList;
        private ObservableCollection<DriveModel> driveList;
        private NavigationContextDirectory _navigationContext;
        private ControlTemplate directoryPanelTemplate;
        private string currentDirectory;
        private object selectedDirectory;
        private FileModel currentFile;

        // Поля из дополнительной части (Tools)
        private IIcon thisComputerIcon;
        private IIcon backButtonIcon;
        private IIcon updateDirectoryPanelIcon;
        private bool thisComputerIconIsEnabled;
        private bool backButtonIsEnabled;
        private bool _refreshScheduled = false;
        private bool openFolderUnderCursorIsEnabled = true;
        /// Флаг, указывающий, что значения плагинов были кэшированы.
        private bool pluginValuesIsCached;
        private IEnumerable<ColumnModel> fileViewColumns;
        private IEnumerable<ColumnModel> folderViewColumns;
        private IEnumerable<ColumnModel> driveViewColumns;
        private IColumnProvider columnProvider;
        private readonly IColumnStateManager columnStateManager;
        private readonly ColumnRegistry columnRegistry;
        private readonly ISettingsStore settings;
        private readonly string PanelId;

        public event Action<string> PathChanged;
        public event Action<string> TabTitleChanged;

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
              IPanelRegistry panelRegistry,
              NavigationContextDirectory navigationContext,
              IGuiCommandExecutor guiCommandExecutor,
              CommandManager manager,
              //ModuleLogger logger,
              //IAppLogger appLogger,
              // <-- NEW dependencies
              IColumnProvider columnProvider,
              IColumnStateManager columnStateManager,
              ISettingsStore settingsStore,
              ColumnRegistry columnRegistry,
              IAppLogger logger) 
            : base(regionManager)
        {
            this._logger = logger.Create(
                category: LogCategory.UserAction,
                scope: LogScope.UserAction
                );

            this.guiCommandExecutor = guiCommandExecutor;
            this._panelRegistry = panelRegistry;
            this._selectionManager = selectionManager;
            this.configService = configService;
            this.dialogService = dialogService;
            this.commandManager = manager;
            //this._appLogger = appLogger;
            this.pluginLoaderService = pluginService;
            this.dataService = dataService;
            this.settingsService = settingsService.GetAppConfig();
            this.globalCommandService = globalCommandService;
            this.multiCommandService = multiCommandService;
            this.multiCommandService.SaveCommand.RegisterCommand(this.SavePanelStateCommand);
            this._panelRegistry = panelRegistry ?? throw new ArgumentNullException(nameof(panelRegistry));

            // Инициализация иконок
            this.ThisComputerIcon = iconProvider.GetIcon(MaterialDesignThemes.Wpf.PackIconKind.LaptopWindows);
            this.BackButtonIcon = iconProvider.GetIcon(MaterialDesignThemes.Wpf.PackIconKind.ArrowBack);
            this.UpdateDirectoryIcon = iconProvider.GetIcon(MaterialDesignThemes.Wpf.PackIconKind.Refresh);
            this.ThisComputerIconIsEnabled = true;
            this.BackButtonIsEnabled = true;
            this._navigationContext = navigationContext;
            this._navigationService = new NavigationManager(null);
            this.DriveList = new ObservableCollection<DriveModel>();

            directoryChangeNotifier.DirectoryChanged += OnDirectoryChanged;
            // заменить локальные new/недостающие поля на внедрённые
            this.columnProvider = columnProvider ?? throw new ArgumentNullException(nameof(columnProvider));
            this.columnStateManager = columnStateManager ?? throw new ArgumentNullException(nameof(columnStateManager));;
            this.settings = settingsStore ?? throw new ArgumentNullException(nameof(settingsStore));
            this.columnRegistry = columnRegistry ?? throw new ArgumentNullException(nameof(settingsStore));
            // подпись на главный обработчик (одно место)
            ColumnSyncService.RegisterHandler("Main", width => OnRemoteWidthChanged("Main", width));
        }

        #endregion

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

        public IReadOnlyList<BaseDirectory> GetFiles() => this.FileList;

        #region Реализация интерфейсов IDirectoryPanel

        /// <summary>
        /// Уникальный токен панели.
        /// </summary>
        public Guid Token { get; set; }

        /// <summary>
        /// Возвращает уникальный токен панели.
        /// </summary>
        /// <returns>Идентификатор панели.</returns>
        public Guid GetPanelToken() => this.Token;

        /// <summary>
        /// Возвращает текущий путь директории.
        /// </summary>
        /// <returns>Путь директории.</returns>
        public string GetCurrentPath() => this.CurrentDirectory;

        /// <summary>
        /// Возвращает путь к текущему выбранному файлу.
        /// </summary>
        /// <returns>Путь к файлу.</returns>
        public string GetCurrentFilePath() => this.CurrentFile?.Path;

        public void SetCurrentPath(string value) => this.CurrentDirectory = value;

        #endregion

        #region Свойства (Properties)

        /// <summary>
        /// Менеджер для управления выделением файлов и папок
        /// </summary>
        public ISelectionManager SelectionManager => this._selectionManager;
        public ISelectionContext SelectionContext => this._selectionContext;
        
        /// <summary>
        /// Текущий путь директории.
        /// </summary>
        public string CurrentDirectory
        {
            get => this.currentDirectory;
            set
            {
                this.SetProperty(ref this.currentDirectory, value);
                PathChanged?.Invoke(currentDirectory);
                var title = PathTitleHelper.GetTabTitle(currentDirectory);
                TabTitleChanged?.Invoke(title);
            }
        }

        /// <summary>
        /// Выбранная директория.
        /// </summary>
        public object SelectedDirectory
        {
            get => this.selectedDirectory;
            set => this.SetProperty(ref this.selectedDirectory, value);
        }

        /// <summary>
        /// Текущий выбранный файл.
        /// При изменении вызывает обновление доступности команд.
        /// </summary>
        public FileModel CurrentFile
        {
            get => this.currentFile;
            set
            {
                this.SetProperty(ref this.currentFile, value);
            }
        }

        /// <summary>
        /// Контекстное меню для файловой панели.
        /// </summary>
        public ContextMenu ContextMenu { get; set; } = new ContextMenu();

        /// <summary>
        /// Контейнер (GridView) для файлов.
        /// </summary>
        public GridView FilePanelContainer { get; set; } = new GridView();

        /// <summary>
        /// Контейнер (GridView) для папок.
        /// </summary>
        public GridView FolderPanelContainer { get; set; } = new GridView();

        /// <summary>
        /// Контейнер (GridView) для дисков.
        /// </summary>
        public GridView DrivePanelContainer { get; set; } = new GridView();

        /// <summary>
        /// Список папок.
        /// </summary>
        public ObservableCollection<FolderModel> DirectoryList
        {
            get => this.directoryList;
            set => this.SetProperty(ref this.directoryList, value);
        }

        /// <summary>
        /// Список файлов.
        /// </summary>
        public ObservableCollection<FileModel> FileList
        {
            get => this.fileList;
            set => this.SetProperty(ref this.fileList, value);
        }

        /// <summary>
        /// Список дисков.
        /// </summary>
        public ObservableCollection<Common.Models.Directory.DriveModel> DriveList
        {
            get => this.driveList;
            set => this.SetProperty(ref this.driveList, value);
        }

        /// <summary>
        /// Шаблон для отображения элементов панели (например, папок).
        /// </summary>
        public ControlTemplate DirectoryPanelTemplate
        {
            get => this.directoryPanelTemplate;
            set => this.SetProperty(ref this.directoryPanelTemplate, value);
        }

        /// <summary>
        /// Выбранный элемент директории.
        /// </summary>
        public BaseDirectory SelectedCurrentDirectoryItem
        {
            get => this.selectedCurrentDirectoryItem;
            set => this.SetProperty(ref this.selectedCurrentDirectoryItem, value);
        }

        #endregion

        #region Свойства из Tools

        /// <summary>
        /// Иконка "Мой компьютер".
        /// </summary>
        public IIcon ThisComputerIcon
        {
            get => this.thisComputerIcon;
            set => this.SetProperty(ref this.thisComputerIcon, value);
        }

        /// <summary>
        /// Иконка кнопки "Назад".
        /// </summary>
        public IIcon BackButtonIcon
        {
            get => this.backButtonIcon;
            set => this.SetProperty(ref this.backButtonIcon, value);
        }

        /// <summary>
        /// Иконка для обновления директории.
        /// </summary>
        public IIcon UpdateDirectoryIcon
        {
            get => this.updateDirectoryPanelIcon;
            set => this.SetProperty(ref this.updateDirectoryPanelIcon, value);
        }

        /// <summary>
        /// Флаг, указывающий, активна ли иконка "Мой компьютер".
        /// </summary>
        public bool ThisComputerIconIsEnabled
        {
            get => this.thisComputerIconIsEnabled;
            set => this.SetProperty(ref this.thisComputerIconIsEnabled, value);
        }

        /// <summary>
        /// Флаг, указывающий, активна ли кнопка "Назад".
        /// </summary>
        public bool BackButtonIsEnabled
        {
            get => this.backButtonIsEnabled;
            set => this.SetProperty(ref this.backButtonIsEnabled, value);
        }

        #endregion

        #region Команды

        /// <summary>
        /// Команда для выбора текущего элемента директории.
        /// </summary>
        public DelegateCommand<BaseDirectory> SelectCurrentDirectoryItem =>
            new DelegateCommand<BaseDirectory>(item => this.SelectedCurrentDirectoryItem = item);

        /// <summary>
        /// Команда для навигации по выбранной папке.
        /// </summary>
        public DelegateCommand<FolderModel> NavigateDirectoryCommand =>
            new DelegateCommand<FolderModel>(dir =>
            {
                if (dir != null)
                {
#if (Nlog)
                    //this.logger.Log(LogLevel.Info, $"Открыта папка ({dir.Path})");
                    _logger.Info($"Открыта папка ({dir.Path})");
#endif
                    _navigationService.TryNavigateTo(dir.Path);
                }
            });

        /// <summary>
        /// Команда для перехода к выбранному диску.
        /// </summary>
        public DelegateCommand<DriveModel> GotoDiskCommand =>
            new DelegateCommand<DriveModel>(dir =>
            {
                if (dir != null)
                {
#if (Nlog)
                    //this.logger.Log(LogLevel.Info, $"Открыт диск ({dir.Letter})");
                    _logger.Info($"Открыт диск ({dir.Letter})");
#endif  
                    _navigationService.TryNavigateTo(dir.Letter);
                }
            });

        /// <summary>
        /// Команда для обновления панели при изменении текущей директории.
        /// </summary>
        public DelegateCommand<object> UpdateCommand =>
            new DelegateCommand<object>(dir =>
            {
                if (dir != null)
                {
#if (Nlog)
                    //this.logger.Log(LogLevel.Info, $"Текущая папка изменена на ({dir})");
                    _logger.Info($"Текущая папка изменена на ({dir})");
#endif
                    _navigationService.TryNavigateTo(dir.ToString(), forceRecord: true);
                }
            });

        /// <summary>
        /// Команда сохранения состояния панели (вызывается при закрытии приложения).
        /// </summary>
        public DelegateCommand SavePanelStateCommand => new DelegateCommand(() =>
        {
            if (settingsService.IsSessionSaved)
            {
                // Логика сохранения состояния панели
            }
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

        /// <summary>
        /// Инициализирует панель, устанавливая уникальный идентификатор и начальное состояние.
        /// Регистрирует команды навигации и устанавливает текущий путь.
        /// </summary>
        /// <param name="token">
        /// Ссылка на токен панели (если null – генерируется новый токен).
        /// </param>
        /// <param name="path">
        /// Начальный путь для отображения.
        /// </param>
        /// <returns>
        /// Инстанс <see cref="ITabPanelContent"/> для текущей панели.
        /// </returns>
        public ITabPanelContent InitializedViewModel(ref Guid token, string path)
        {
            this.CurrentDirectory = path;

            // Если токен не задан, создаём новый
            if (token == Guid.Empty)
                token = Guid.NewGuid();

            this.Token = token;

            // Регистрируем его в глобальном реестре
            NavigationContextDirectory.Instance.Register(this.Token, _navigationService);

            // Подписка на изменения пути
            _navigationService.CurrentChanged += OnPathChanged;
            
            // Восстановление состояния панели из предыдущего сеанса
            _ = this.SetLastPanelState();

            _adapter = new PanelViewModelAdapter(this);
            _panelRegistry.RegisterPanel(_adapter);


            if (FileList != null)
                _selectionContext = new SelectionContext(FileList.Cast<ISelectableItem>());

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
                FileList = new ObservableCollection<FileModel>();

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
                DirectoryList = new ObservableCollection<FolderModel>();

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
            // Загружаем параллельно
            var dirsTask = dataService.GetDirectoriesAsync(dirPath);
            var filesTask = dataService.GetFilesAsync(dirPath);

            var dirs = await dirsTask;
            var files = await filesTask;

            RefreshDirectoryList(dirs);
            RefreshFileList(files);

            this.CurrentDirectory = dirPath;
            // 🔥 Асинхронный пересчёт колонок
            await UpdateColumnValuesAsync();
        }

        #endregion

        #endregion

        #region Работа с плагинными колонками

        /// <summary>
        /// Добавляет дополнительные колонки, предоставляемые плагинами.
        /// </summary>
        private void AddPluginColumns()
        {
            foreach (var pluginContext in this.pluginLoaderService.GetPluginContext())
            {
                foreach (var column in pluginContext.GetColumns())
                {
                    column.ColumnManager.SetUpdateCommand(this.UpdateColumnsCommand);
                    column.ColumnBuilder.UpdateColumnValue(column.ColumnManager);

                    // Инициализация колонок для папок
                    if (this.InitialFolderColumnValues(column))
                    {
                        var gridColumn = new GridViewColumn
                        {
                            Header = column.Header ?? "Error",
                            Width = column.Width,
                            DisplayMemberBinding = new Binding($"Additional[{column.Header}]")
                        };
                        column.ColumnManager.SetHideCommand(this.HideColumnCommand, this.AddColumnCommand, gridColumn);
                        this.FolderPanelContainer.Columns.Add(gridColumn);
                    }

                    // Инициализация колонок для файлов
                    if (this.InitialFileColumnValues(column))
                    {
                        var gridColumn = new GridViewColumn
                        {
                            Header = column.Header ?? "Error",
                            Width = column.Width,
                            CellTemplate = (DataTemplate)Application.Current.FindResource("ColumnTextDataTemplate")
                        };
                        this.FilePanelContainer.Columns.Add(gridColumn);
                    }

                    this.CreatePluginContextMenu(column);
                }
            }
        }

        /// <summary>
        /// Создаёт контекстное меню для колонки, предоставляемой плагином.
        /// </summary>
        /// <param name="column">Объект колонки.</param>
        private void CreatePluginContextMenu(IColumn column)
        {
            if (column.ContextItems == null) return;

            foreach (var item in column.ContextItems)
            {
                this.ContextMenu.Items.Add(new MenuItem().SetParam(item.Command, paramManager =>
                {
                    paramManager.AddParam(this, "CurrentDirectory");
                }));
            }
        }

        /// <summary>
        /// Обновляет плагинные колонки для папок и файлов.
        /// </summary>
        private async void UpdateColumnsCommand()
        {
            // Выполним в пуле потоков, чтобы не блокировать UI
            await Task.Run(() =>
            {
                foreach (var pluginContext in this.pluginLoaderService.GetPluginContext())
                {
                    foreach (var column in pluginContext.GetColumns())
                    {
                        foreach (var folder in this.DirectoryList)
                        {
                            var value = column.ColumnBuilder.ColumnValueHandler(column.Header, folder.Path, DirectoryItemType.Folder);
                            if (value != null)
                            {
                                // UI-операции отложим
                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    folder.Additional[column.Header] = value;
                                });
                            }
                        }

                        foreach (var file in this.FileList)
                        {
                            var value = column.ColumnBuilder.ColumnValueHandler(column.Header, file.Path, DirectoryItemType.File);
                            if (value != null)
                            {
                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    file.Additional[column.Header] = value;
                                });
                            }
                        }
                    }
                }
            }).ConfigureAwait(false); // Не вернёмся в UI-поток, оставим фоновым
        }

        /// <summary>
        /// Инициализирует дополнительные значения колонок для папок.
        /// </summary>
        /// <param name="column">Колонка, предоставляемая плагином.</param>
        /// <returns>True, если для всех папок установлены значения.</returns>
        private bool InitialFolderColumnValues(IColumn column)
        {
            foreach (var folder in this.DirectoryList)
            {
                if (!folder.Additional.ContainsKey(column.Header))
                {
                    folder.Additional.Add(column.Header, column.ColumnBuilder.ColumnValueHandler(column.Header, folder.Path, DirectoryItemType.Folder));
                }
            }
            return true;
        }

        /// <summary>
        /// Инициализирует дополнительные значения колонок для файлов.
        /// </summary>
        /// <param name="column">Колонка, предоставляемая плагином.</param>
        /// <returns>True, если для всех файлов установлены значения.</returns>
        private bool InitialFileColumnValues(IColumn column)
        {
            foreach (var file in this.FileList)
            {
                if (!file.Additional.ContainsKey(column.Header))
                {
                    file.Additional.Add(column.Header, column.ColumnBuilder.ColumnValueHandler(column.Header, file.Path, DirectoryItemType.File));
                }
            }
            return true;
        }

        /// <summary>
        /// Команда для скрытия указанной колонки.
        /// </summary>
        /// <param name="column">Объект колонки.</param>
        private void HideColumnCommand(DependencyObject column)
        {
            if (this.FolderPanelContainer.Columns.Contains(column))
            {
                this.FolderPanelContainer.Columns.Remove((GridViewColumn)column);
            }
        }

        /// <summary>
        /// Команда для добавления указанной колонки.
        /// </summary>
        /// <param name="column">Объект колонки.</param>
        private void AddColumnCommand(DependencyObject column)
        {
            if (!this.FolderPanelContainer.Columns.Contains(column))
            {
                this.FolderPanelContainer.Columns.Add((GridViewColumn)column);
            }
        }

        /// <summary>
        /// Создаёт контекстное меню для файловой панели с глобальными командами.
        /// </summary>
        private void CreateContextMenu()
        {
            var globalCommands = new List<GlobalCommand>
            {
                new GlobalCommand { DisplayName = CommandNames.ContentViewer, Name = CommandNames.ContentViewer },
                new GlobalCommand { DisplayName = "Create", Name = CommandNames.FileCreate },
                new GlobalCommand { DisplayName = "Delete", Name = CommandNames.FileDelete },
                new GlobalCommand { DisplayName = "Move", Name = CommandNames.FileMove },
                new GlobalCommand { DisplayName = "Directory Update", Name = CommandNames.DirectoryUpdate },
            };

            foreach (var command in globalCommands)
            {
                this.ContextMenu.Items.Add(new MenuItem().SetParam(
                    command,
                    paramManager =>
                    {
                        paramManager.AddParam(this, "SelectedCurrentDirectoryItem.Path");
                        paramManager.AddParam(this, "SelectedCurrentDirectoryItem.Path");
                    }));
            }
        }

        #endregion

        #region Управление ресурсами и навигация

        /// <summary>
        /// Восстанавливает состояние панели из предыдущего сеанса, устанавливает шаблон, загружает файлы и папки, и инициализирует колонки.
        /// </summary>
        private async Task SetLastPanelState()
        {
            // Зарегистрируй провайдеры через DI, если нужно (если у тебя columnRegistry используешь — можно оставить)
            //columnRegistry.RegisterProvider(new DefaultColumnProvider()); // можно убрать если провайдер инжектируется

            // Для текущего пути — если MyComputer — грузим диски
            if (this.CurrentDirectory == VirtualPaths.MyComputer)
            {
                DirectoryPanelTemplate = (ControlTemplate)Application.Current.FindResource("DriveListViewTemplate");
                DriveList = new ObservableCollection<DriveModel>();
                var drives = await dataService.GetDrivesAsync();
                foreach (var dr in drives) DriveList.Add(dr);
            }
            else
            {
            }
            this.DirectoryPanelTemplate = (ControlTemplate)Application.Current.FindResource("DirectoryListViewTemplate");

            // Загружаем пустые коллекции
            FileList = new ObservableCollection<FileModel>();
            DirectoryList = new ObservableCollection<FolderModel>();

            try
            {
                if (this.CurrentDirectory != VirtualPaths.MyComputer)
                {
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

            // Загрузить определения колонок и состояние через state manager
            var defsFiles = columnRegistry.GetColumns(PanelType.Files).ToList();
            var defsFolders = columnRegistry.GetColumns(PanelType.Folders).ToList();
            var defsDrives = columnRegistry.GetColumns(PanelType.Drives).ToList();

            FileViewColumns = columnStateManager.LoadState("LeftPanel.Files", PanelType.Files, defsFiles);
            FolderViewColumns = columnStateManager.LoadState("LeftPanel.Folders", PanelType.Folders, defsFolders);
            DriveViewColumns = columnStateManager.LoadState("LeftPanel.Drives", PanelType.Drives, defsDrives);

            // Теперь инициализируем колонки (но не создаём GridViewColumn в VM)
            await UpdateColumnValuesAsync();
        }

        /// <summary>
        /// Переключает панель в режим отображения дисков.
        /// </summary>
        /// <param name="root">Путь к корневой директории (например, "C:\").</param>
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

        /// <summary>
        /// Обработчик изменения состояния выполнения команды навигации.
        /// Обновляет доступность иконок в зависимости от текущего состояния.
        /// </summary>
        /// <param name="obj">Объект команды с данными навигации.</param>
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

        /// <summary>
        /// Освобождает ресурсы и отсоединяет команды для предотвращения утечек памяти.
        /// </summary>
        public override void Destroy()
        {
            _navigationService.CurrentChanged -= OnPathChanged;
            //columnSync.ColumnChanged -= OnColumnChanged;
            _panelRegistry.UnregisterPanel(_adapter.PanelId);
            this.multiCommandService.SaveCommand.UnregisterCommand(this.SavePanelStateCommand);
            base.Destroy();
            
        }

        #endregion

        #region Методы для обновления панели файлов

        private void OnDirectoryChanged(string changedPath)
        {
            // TODO Сделать более надежный способ для определения пути для целевой панели, пока работает стабльно но может сломаться
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

        /// <summary>
        /// Команда для возврата к предыдущей директории.
        /// Если возможен откат, выполняется переход на предыдущий уровень навигации.
        /// </summary>
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

        /// <summary>
        /// Команда для перехода на панель дисков.
        /// Выполняет команду первого элемента навигационного стека, если он указывает на "Root:".
        /// </summary>
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
                    // Обновляем диски
                    _ = GoDrivePanel();
                }
            });

        /// <summary>
        /// Команда обновления панели с использованием глобальных команд.
        /// </summary>
        public DelegateCommand<object> UpdateDirectoryPanelCommand =>
            new DelegateCommand<object>(obj =>
            {
                var globalCommandManager = globalCommandService.GetCommandManager();
                var cmd = globalCommandManager.GetCommand(CommandNames.DirectoryUpdate);
                cmd.Command?.Execute(null);
            });

        // Пример команды для перехода
        public void NavigateTo(string? path)
        {
            if (_navigationService.IsValidPath(path))
                _navigationService.TryNavigateTo(path);
        }

        #endregion
    }
}
