// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SplitPanelViewModel.cs" company="T">
//   Copyright (c) Alexei Kuznecov. All rights reserved.
// </copyright>
// <summary>
//   Реализация ViewModel для левой панели файлового менеджера. 
//   Обрабатывает навигацию, перетаскивание (drag & drop), работу с плагинами и обновление колонок.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using CommandSystem.Core.Commands;
using CommandSystem.Core.Metadata;
using CommandSystem.Gui.Integraion;
using NLog;
using Prism.Commands;
using Prism.Regions;
using Prism.Services.Dialogs;
using UnityCommander.Common;
using UnityCommander.Common.Commands;
using UnityCommander.Common.Models.Directory;
using UnityCommander.Common.Models.Icons;
using UnityCommander.Common.Module;
using UnityCommander.Core;
using UnityCommander.Core.Commands;
using UnityCommander.Core.Commands.Base;
using UnityCommander.Core.DragDrop;
using UnityCommander.Core.Helper;
using UnityCommander.Core.Mvvm;
using UnityCommander.Integration.Columns;
using UnityCommander.Integration.Commands;
using UnityCommander.Integration.Enums;
using UnityCommander.Modules.FilePanel.Columns;
using UnityCommander.Services.Interfaces;
using CommandManager = UnityCommander.Core.Commands.CommandManager;

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
        private readonly GuiCommandRegistrar commandRegistered;
        private readonly GuiCommandExecute commandExecute;
        private readonly IAppConfigService configService;
        private readonly CommandManager commandManager;
        private readonly ModuleLogger logger;

        /// <summary>
        /// Флаг, указывающий, что значения плагинов были кэшированы.
        /// </summary>
        private bool pluginValuesIsCached;

        // --- Прочие поля
        private BaseDirectory selectedCurrentDirectoryItem;
        private ObservableCollection<FileModel> fileList;
        private ObservableCollection<FolderModel> directoryList;
        private ObservableCollection<DriveModel> driveList;
        private NavigationInvoker navigationCommand;
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
        private bool openFolderUnderCursorIsEnabled = true;

        public event Action<string> PathChanged;

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
            GuiCommandRegistrar guiCommandRegistrar,
            GuiCommandExecute guiCommandExecute,
            CommandManager manager,
            ModuleLogger logger)
            : base(regionManager)
        {
            this.commandRegistered = guiCommandRegistrar;
            this.commandExecute = guiCommandExecute;
            this.configService = configService;
            this.dialogService = dialogService;
            this.commandManager = manager;
            this.logger = logger;
            this.pluginLoaderService = pluginService;
            this.dataService = dataService;
            this.settingsService = settingsService.GetAppConfig();
            this.globalCommandService = globalCommandService;
            this.multiCommandService = multiCommandService;
            this.multiCommandService.SaveCommand.RegisterCommand(this.SavePanelStateCommand);
            
            // Инициализация иконок
            this.ThisComputerIcon = iconProvider.GetIcon(MaterialDesignThemes.Wpf.PackIconKind.LaptopWindows);
            this.BackButtonIcon = iconProvider.GetIcon(MaterialDesignThemes.Wpf.PackIconKind.ArrowBack);
            this.UpdateDirectoryIcon = iconProvider.GetIcon(MaterialDesignThemes.Wpf.PackIconKind.Refresh);
            this.ThisComputerIconIsEnabled = true;
            this.BackButtonIsEnabled = true;
        }

        #endregion

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
        /// Текущий путь директории.
        /// </summary>
        public string CurrentDirectory
        {
            get => this.currentDirectory;
            set 
            { 
                this.SetProperty(ref this.currentDirectory, value);
                PathChanged?.Invoke(currentDirectory);
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
                this.navigationCommand?.RaiseExecuteChanged(this);
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
        public ObservableCollection<DriveModel> DriveList
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
        /// Пример тестовой команды.
        /// </summary>
        public ICommand TestCommand { get; set; }

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
                    this.logger.Log(LogLevel.Info, $"Открыта папка ({dir.Path})");
#endif                  
                    //var currentPath = this.commandExecute.ExecuteAsync("setcurpath", dir.Path);
                    navigationCommand.Execute(UpdateFilePanel, dir.Path);
                    //PathChanged?.Invoke(dir.Path);
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
                    this.logger.Log(LogLevel.Info, $"Открыт мой компьютер ({dir.Letter})");
#endif
                    navigationCommand.Execute(UpdateFilePanel, dir.Letter);
                    //PathChanged?.Invoke(dir.ToString());
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
                    this.logger.Log(LogLevel.Info, $"Текущая папка изменена на ({dir})");
#endif
                    navigationCommand.Execute(UpdateFilePanel, dir);
                    //PathChanged?.Invoke(dir.ToString());
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
            bool isMultiSelect = dropInfo.Data is List<object> && dropInfo.TargetItem is ListBox or BaseDirectory;
            bool isSingleSelect = dropInfo.Data is BaseDirectory && dropInfo.TargetItem is ListBox or BaseDirectory;

            var adorner = AdornerLayer.GetAdornerLayer(dropInfo.VisualTarget);
            if (adorner == null)
                this.CreateAdornerLayer(dropInfo.VisualTarget);

            if (isMultiSelect || isSingleSelect)
                dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;

            if (dropInfo.Data is BaseDirectory && dropInfo.VisualTarget is ListBox && dropInfo.TargetItem == null)
                dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;

            dropInfo.Effects = DragDropEffects.Copy;
        }

        /// <summary>
        /// Обрабатывает событие Drop, инициируя диалог копирования и передачу параметров.
        /// </summary>
        /// <param name="dropInfo">Информация о событии Drop.</param>
        void IDropTarget.Drop(IDropInfo dropInfo)
        {
            BaseDirectory sourceItem = dropInfo.Data as BaseDirectory;
            BaseDirectory targetItem = dropInfo.TargetItem as BaseDirectory;
            string targetPath = null;
            var visualTarget = dropInfo.VisualTarget as ListBox;
            var splitPanelViewModel = visualTarget.DataContext as SplitPanelViewModel;

            if (targetItem == null)
            {
                var firstItem = visualTarget?.SelectedItem as BaseDirectory;
                if (firstItem != null)
                {
                    var pathParts = firstItem.Path.Split('\\');
                    targetPath = Path.Combine(pathParts.Take(pathParts.Length - 1).ToArray());
                }
                else
                {
                    targetPath = this.CurrentDirectory;
                }
            }

            this.dialogService.ShowDialog("CopyDialog",
                new OverrideDialogParameters(new CopyParameters
                {
                    Source = (dropInfo.Data as BaseDirectory)?.Path,
                    Target = (dropInfo.TargetItem as BaseDirectory)?.Path ?? targetPath
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
            this.Token = token == null ? Guid.NewGuid() : token;

            // Инициализация навигационного инвокера через менеджер команд
            var invoker = new NavigationInvoker();
            this.navigationCommand = (NavigationInvoker)this.commandManager.CommandRegister(token, invoker);

            // Восстановление состояния панели из предыдущего сеанса
            this.SetLastPanelState();

            // Если путь не задан и разрешено открывать папку под курсором, получить путь из команды
            if (path == null && this.openFolderUnderCursorIsEnabled)
            {
                var command = this.navigationCommand.GetCommand();
                this.CurrentDirectory = command.GetPath();
                this.SetCommands(this.CurrentDirectory);
            }
            else
            {
                this.SetCommands(path);
            }

            return this;
        }

        /// <summary>
        /// Обнавляет панель файлов, как правило метод вызвается из вне.
        /// </summary>
        /// <param name="directoryPanel"></param>
        public void DirectoryUpdate(IDirectoryPanel directoryPanel)
        {
            // Реализация логики по обнавлению директории.
        }

        #endregion

        #region Управление колонками и плагинами

        /// <summary>
        /// Инициализирует колонки панели, включая стандартные и плагинные, а также настраивает контекстное меню.
        /// </summary>
        private void InitializeColumns()
        {
            this.AddFileColumns();
            this.AddFolderColumns();
            this.AddDriveColumns();
            //this.AddPluginColumns();
            this.CreateContextMenu();
            this.navigationCommand = (NavigationInvoker)this.commandManager.GetCommand(this.Token);
            this.navigationCommand.OnExecuteChanged += this.OnExecuteChanged;
        }

        /// <summary>
        /// Добавляет колонки для папок.
        /// </summary>
        private void AddFolderColumns()
        {
            new FolderColumnModel().GetColumn((columns, error) =>
            {
                foreach (var col in columns)
                {
                    this.FolderPanelContainer.Columns.Add((GridViewColumn)col.Template);
                }
            });
        }

        /// <summary>
        /// Добавляет колонки для файлов.
        /// </summary>
        private void AddFileColumns()
        {
            new FileColumnModel().GetColumn((columns, error) =>
            {
                foreach (var col in columns)
                {
                    this.FilePanelContainer.Columns.Add((GridViewColumn)col.Template);
                }
            });
        }

        /// <summary>
        /// Добавляет колонки для отображения дисков.
        /// </summary>
        private void AddDriveColumns()
        {
            new DriveContainerModel().GetColumn((columns, error) =>
            {
                foreach (var col in columns)
                {
                    this.DrivePanelContainer.Columns.Add((GridViewColumn)col.Template);
                }
            });
        }

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

        #endregion

        #region Управление ресурсами и навигация

        /// <summary>
        /// Восстанавливает состояние панели из предыдущего сеанса, устанавливает шаблон, загружает файлы и папки, и инициализирует колонки.
        /// </summary>
        private async void SetLastPanelState()
        {
            this.DirectoryPanelTemplate = (ControlTemplate)Application.Current.FindResource("DirectoryListViewTemplate");
            var files = await dataService.GetFilesAsync(this.CurrentDirectory);
            var dirs = await dataService.GetDirectoriesAsync(this.CurrentDirectory);
            FileList = new ObservableCollection<FileModel>();
            foreach (var f in files)
                FileList.Add(f);
            DirectoryList = new ObservableCollection<FolderModel>();
            foreach (var d in dirs)
                DirectoryList.Add(d);
            this.InitializeColumns();
        }

        /// <summary>
        /// Обновляет файловую панель: устанавливает новый шаблон, загружает директории и файлы, обновляет текущий путь,
        /// а затем обновляет плагинные колонки.
        /// </summary>
        /// <param name="dirPath">Путь к новой директории.</param>
        private async void UpdateFilePanel(object dirPath)
        {
            var template = (ControlTemplate)Application.Current.FindResource("DirectoryListViewTemplate");
            if (!this.DirectoryPanelTemplate.Equals(template))
            {
                this.DirectoryPanelTemplate = template;
            }
            var path = Directory.Exists(dirPath.ToString()) ? dirPath.ToString() : Directory.GetDirectoryRoot(dirPath.ToString());
            var files = await dataService.GetFilesAsync(path);
            var dirs = await dataService.GetDirectoriesAsync(path);
            FileList = new ObservableCollection<FileModel>();
            foreach (var f in files)
                FileList.Add(f);
            DirectoryList = new ObservableCollection<FolderModel>();
            foreach (var d in dirs)
                DirectoryList.Add(d);

            this.CurrentDirectory = path;
            this.UpdateColumnsCommand(); // Обновление плагинных колонок
        }

        /// <summary>
        /// Переключает панель в режим отображения дисков.
        /// </summary>
        /// <param name="root">Путь к корневой директории (например, "C:\").</param>
        private async Task GoDrivePanel(object root)
        {
            this.DirectoryPanelTemplate = (ControlTemplate)Application.Current.FindResource("DriveListViewTemplate");
            var drivers = await this.dataService.GetDrivesAsync();
            foreach (var d in drivers)
                DriveList.Add(d);
            this.CurrentDirectory = (string)root;
        }

        /// <summary>
        /// Регистрирует команды навигации по директориям.
        /// </summary>
        /// <param name="dirPath">Путь к директории.</param>
        private void SetCommands(string dirPath)
        {
            var paths = HelperFunctions.ParsePath(dirPath);
            this.navigationCommand.AddCommand(this.GoDrivePanel, "Root:C:\\");
            foreach (var path in paths)
            {
                this.navigationCommand.AddCommand(this.UpdateFilePanel, path);
            }
        }

        #endregion

        #region Обработка событий и очистка ресурсов

        /// <summary>
        /// Обработчик изменения состояния выполнения команды навигации.
        /// Обновляет доступность иконок в зависимости от текущего состояния.
        /// </summary>
        /// <param name="obj">Объект команды с данными навигации.</param>
        private void OnExecuteChanged(object obj)
        {
            if (obj is ConcreteCommand { Receiver: Navigator navigator })
            {
                if (!navigator.Path.Contains("Root:"))
                {
                    this.ThisComputerIconIsEnabled = true;
                    this.BackButtonIsEnabled = true;
                }
            }
        }

        /// <summary>
        /// Освобождает ресурсы и отсоединяет команды для предотвращения утечек памяти.
        /// </summary>
        public override void Destroy()
        {
            base.Destroy();
            this.multiCommandService.SaveCommand.UnregisterCommand(this.SavePanelStateCommand);
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
                if (this.navigationCommand.CanUndo)
                {
                    this.navigationCommand.Previous();
                }
                if (!this.navigationCommand.CanUndo)
                {
                    this.BackButtonIsEnabled = false;
                    this.ThisComputerIconIsEnabled = false;
                }
            });

        /// <summary>
        /// Команда для перехода на панель дисков.
        /// Выполняет команду первого элемента навигационного стека, если он указывает на "Root:".
        /// </summary>
        public DelegateCommand<object> GoDrivePanelCommand =>
            new DelegateCommand<object>(obj =>
            {
                var command = (ConcreteCommand)this.navigationCommand.FirstCommand;
                if (command?.Receiver is Navigator navigator)
                {
                    if (navigator.Path.Contains("Root:"))
                    {
                        navigator.CommandArg.Invoke(navigator.Path);
                    }
                }
                this.ThisComputerIconIsEnabled = false;
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

        /// <summary>
        /// Обработчик изменения состояния глобальных команд.
        /// Обновляет состояние иконок на основе текущего пути навигации.
        /// </summary>
        /// <param name="obj">Объект навигационных данных.</param>
        private void OnExecuteChangedTools(object obj)
        {
            if (obj is ConcreteCommand { Receiver: Navigator navigator })
            {
                if (!navigator.Path.Contains("Root:"))
                {
                    this.ThisComputerIconIsEnabled = true;
                    this.BackButtonIsEnabled = true;
                }
            }
        }

        #endregion
    }
}
