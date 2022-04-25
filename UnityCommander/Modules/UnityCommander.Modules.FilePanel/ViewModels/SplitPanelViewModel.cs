// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SplitPanelViewModel.cs" company="T">
//   Copyright (p) Alexey Kuznecov. All right reserved.
// </copyright>
// <summary>
//   The left panel view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using NLog;
using Prism.Commands;
using Prism.Regions;
using Prism.Services.Dialogs;
using UnityCommander.Common;
using UnityCommander.Common.Models.Columns;
using UnityCommander.Common.Models.Directory;
using UnityCommander.Core;
using UnityCommander.Core.Commands;
using UnityCommander.Core.DragDrop;
using UnityCommander.Core.Helper;
using UnityCommander.Core.IO.Operations;
using UnityCommander.Core.Modules;
using UnityCommander.Core.Mvvm;
using UnityCommander.Integration.Columns;
using UnityCommander.Integration.Enums;
using UnityCommander.Services.Interfaces;
using CommandManager = UnityCommander.Core.Commands.CommandManager;

namespace UnityCommander.Modules.FilePanel.ViewModels
{
    /// <summary>
    /// The left panel view model.
    /// </summary>
    [Serializable]
    public class SplitPanelViewModel : RegionViewModelBase, IDropTarget, IDirectoryPanel
    {
        #region Declaration fields

        #region Dependencies Injection

        /// <summary>
        /// The dialog service.
        /// </summary>
        private readonly IDialogService dialogService;

        /// <summary>
        /// The directory provider.
        /// </summary>
        private readonly IDataProviderService dataService;

        /// <summary>
        /// The application settings.
        /// </summary>
        private readonly ISettings settingsService;

        /// <summary>
        /// The common state service.
        /// </summary>
        private readonly IMultiCommandService multiCommandService;

        /// <summary>
        /// The common state service.
        /// </summary>
        private readonly IGlobalCommandService globalCommandService;

        /// <summary>
        /// The plugin loader service.
        /// </summary>
        private readonly IPluginLoaderService pluginLoaderService;
        
        /// <summary>
        /// The command manager.
        /// </summary>
        private readonly CommandManager commandManager;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly Logger logger;

        /// <summary>
        /// If true, the plugin was cached and the result will be restored
        /// from the cache table the next time the program starts.
        /// </summary>
        private bool pluginValuesIsCached;

        /// <summary>
        /// Select base directory.
        /// </summary>
        private BaseDirectory selectedBaseDirectory;

        #endregion

        #region Collections

        /// <summary>
        /// The file list.
        /// </summary>
        private ObservableCollection<FileModel> fileList;

        /// <summary>
        /// The directory list.
        /// </summary>
        private ObservableCollection<FolderModel> directoryList;

        /// <summary>
        /// The drive list.
        /// </summary>
        private ObservableCollection<DriveModel> driveList;

        #endregion

        /// <summary>
        /// The navigation command.
        /// </summary>
        private NavigationInvoker navigationCommand;
        
        /// <summary>
        /// Control template for panel items.
        /// </summary>
        private ControlTemplate directoryPanelTemplate;
        
        /// <summary>
        /// Indicates the current directory.
        /// </summary>
        private string currentDirectory;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SplitPanelViewModel"/> class.
        /// </summary>
        /// <param name="dialogService">
        /// The dialog service.
        /// </param>
        /// <param name="regionManager">
        /// The region manager is Prism implementation.
        /// </param>
        /// <param name="settingsService">
        /// The service that provides interface to configure of application.
        /// </param>
        /// <param name="dataService">
        /// The service that provides the info of the file system items.
        /// </param>
        /// <param name="multiCommandService">
        /// The service that respond for composite commands.
        /// </param>
        /// <param name="pluginService">
        /// The service for loading all detected plugin interfaces.
        /// </param>
        /// <param name="manager">
        /// Command manager
        /// </param>
        /// <param name="logger">
        /// Log manager.
        /// </param>
        public SplitPanelViewModel(
            IDialogService dialogService,
            IRegionManager regionManager,
            ISettingsProviderService settingsService,
            IDataProviderService dataService,
            IMultiCommandService multiCommandService,
            IGlobalCommandService globalCommandService,
            IPluginLoaderService pluginService,
            CommandManager manager,
            ModuleLogger logger)
            : base(regionManager)
        {
            this.dialogService = dialogService;
            commandManager = manager;
            this.logger = logger.GetLogger();
            pluginLoaderService = pluginService;
            this.dataService = dataService;
            this.settingsService = settingsService.GetAppConfig();

            this.globalCommandService = globalCommandService;

            var fileManger = this.globalCommandService.GetCommandManager<FileManager>();
            this.TestCommand = fileManger.GetCommand(CommandNames.FileMove);
            
            // Composite command
            this.multiCommandService = multiCommandService;
            this.multiCommandService.SaveCommand.RegisterCommand(SavePanelStateCommand);
        }

        #endregion

        #region Commands

        /// <summary>
        /// Goes to the selected directory.
        /// </summary>
        public ICommand TestCommand { get; set; }

        /// <summary>
        /// Goes to the selected directory.
        /// </summary>
        public DelegateCommand<FolderModel> NavigateDirectoryCommand => new DelegateCommand<FolderModel>(
            dir =>
            {
                if (dir != null)
                {
#if (Nlog)
                    logger.Info("File Panel: '{0}'", dir.Path);
#endif
                    navigationCommand.Execute(UpdateFilePanel, dir.Path);
                }
            });

        /// <summary>
        /// Goes to the selected disk.
        /// </summary>
        public DelegateCommand<DriveModel> GotoDiskCommand => new DelegateCommand<DriveModel>(
            dir =>
            {
                if (dir != null)
                {
#if (Nlog)
                    logger.Info("Driv Panel: '{0}'", dir.Letter);
#endif
                    navigationCommand.Execute(UpdateFilePanel, dir.Letter);
                }
            });

        /// <summary>
        /// Gets or sets a navigate command.
        /// </summary>
        public DelegateCommand<object> UpdateCommand => new DelegateCommand<object>(
            dir =>
            {
                if (dir != null)
                {
#if (Nlog)
                    logger.Info("Navi Panel: '{0}'", dir);
#endif
                    navigationCommand.Execute(UpdateFilePanel, dir);
                }
            });

        /// <summary>
        /// The command serializes the state of the file panel after the program is closed
        /// to restore it to its original state the next time it starts. <see cref="SetLastPanelState"/>
        /// </summary>
        public DelegateCommand SavePanelStateCommand => new DelegateCommand(
            () =>
            {
                if (settingsService.IsSessionSaved)
                {
                }
            });

        #endregion

        #region Delaration properties

        /// <summary>
        /// Gets or sets the current directory.
        /// </summary>
        public string CurrentDirectory
        {
            get => currentDirectory;
            set => SetProperty(ref currentDirectory, value);
        }

        /// <summary>
        /// Gets or sets grid view for file panel.
        /// </summary>
        public ContextMenu ContextMenu { get; set; } = new ();

        /// <summary>
        /// Gets or sets grid view for file panel.
        /// </summary>
        public GridView FilePanelContainer { get; set; } = new ();

        /// <summary>
        /// Gets or sets grid view for folder panel.
        /// </summary>
        public GridView FolderPanelContainer { get; set; } = new ();

        /// <summary>
        /// Gets or sets grid view for folder panel.
        /// </summary>
        public GridView DrivePanelContainer { get; set; } = new ();

        #region Collection Data

        /// <summary>
        /// Gets or sets the directory list.
        /// </summary>
        public ObservableCollection<FolderModel> DirectoryList
        {
            get => directoryList;
            set => SetProperty(ref directoryList, value);
        }

        /// <summary>
        /// Gets or sets the file list.
        /// </summary>
        public ObservableCollection<FileModel> FileList
        {
            get => fileList;
            set => SetProperty(ref fileList, value);
        }

        /// <summary>
        /// Gets or sets the file list.
        /// </summary>
        public ObservableCollection<DriveModel> DriveList
        {
            get => driveList;
            set => SetProperty(ref driveList, value);
        }

        #endregion

        /// <summary>
        /// Gets or sets the template for panel items.
        /// </summary>
        public ControlTemplate DirectoryPanelTemplate
        {
            get => directoryPanelTemplate;
            set => SetProperty(ref directoryPanelTemplate, value);
        }

        /// <summary>
        /// Sets the selected directory.
        /// </summary>
        public BaseDirectory SelectedBaseDirectory
        {
            set
            {
                if (value != null)
                {
                    SelectedDirectories.Add(value);
                }
            }
        }

        /// <summary>
        /// Sets the selected directory.
        /// </summary>
        public BaseDirectory CurrentFile
        {
            get => selectedBaseDirectory;
            set => selectedBaseDirectory = value;
        }

        /// <summary>
        /// Gets or sets the selected directory.
        /// </summary>
        public List<BaseDirectory> SelectedDirectories { get; set; } = new ();

        /// <summary>
        /// Gets or sets the token.
        /// </summary>
        public Guid Token { get; set; }

        /// <summary>
        /// The initial panel.
        /// </summary>
        /// <param name="token">
        /// The token.
        /// </param>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <returns>
        /// The <see cref="IDirectoryPanel"/>.
        /// </returns>
        public IDirectoryPanel InitializedViewModel(Guid token, string path)
        {
#if (Nlog)
            logger.Info("Token: {0} Path: {1}", token, path);
#endif
            CurrentDirectory = path;
            Token = token;
            navigationCommand = (NavigationInvoker)commandManager.CommandRegister(token, new NavigationInvoker());
            SetLastPanelState();
            SetCommands(path);
            return this;
        }

        /// <summary>
        /// The get panel token.
        /// </summary>
        /// <returns>
        /// The <see cref="Guid"/>.
        /// </returns>
        public Guid GetPanelToken() => Token;

        /// <summary>
        /// The get panel token.
        /// </summary>
        /// <returns>
        /// The <see cref="Guid"/>.
        /// </returns>
        public string GetCurrentPath() => CurrentDirectory;

        #endregion

        #region Other
        
        /// <summary>
        /// Finalization of objects when unloading a module.
        /// </summary>
        public override void Destroy()
        {
            base.Destroy();

            // Detaching a command to avoid memory leaks.
            multiCommandService.SaveCommand.UnregisterCommand(SavePanelStateCommand);
        }

        #endregion

        #region Drag And Drop Handlers

        /// <summary>
        /// The drag over.
        /// </summary>
        /// <param name="dropInfo">
        /// The drop-over event handler.
        /// </param>
        void IDropTarget.DragOver(IDropInfo dropInfo)
        {
            bool isMultiSelect = dropInfo.Data is List<object>
                                 & (dropInfo.TargetItem is ListBox || dropInfo.TargetItem is BaseDirectory);
            bool isSingleSelect = dropInfo.Data is BaseDirectory
                                  & (dropInfo.TargetItem is ListBox || dropInfo.TargetItem is BaseDirectory);

            var adorner = AdornerLayer.GetAdornerLayer(dropInfo.VisualTarget);

            if (adorner == null)
            {
                CreateAdornerLayer(dropInfo.VisualTarget);
            }


            if (isMultiSelect || isSingleSelect)
            {
                // var list = dropInfo.VisualTarget as ListView;
                // var template = Application.Current.FindResource("DragAdorner");
                // list?.SetValue(DragDrop.DragAdornerTemplateProperty, template);
                dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                
            }

            if (dropInfo.Data is BaseDirectory & dropInfo.VisualTarget is ListBox & dropInfo.TargetItem == null)
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
            }

            dropInfo.Effects = DragDropEffects.Copy;
        }

        /// <summary>
        /// The drop.
        /// </summary>
        /// <param name="dropInfo">
        /// The drop event handler.
        /// </param>
        /// [DebuggerStepThrough]
        void IDropTarget.Drop(IDropInfo dropInfo)
        {
            BaseDirectory sourceItem = dropInfo.Data as BaseDirectory;
            BaseDirectory targetItem = dropInfo.TargetItem as BaseDirectory;
            string targetPath = null;

            if (targetItem is null)
            {
                var firstItem = (dropInfo.VisualTarget as ListBox).SelectedItem as BaseDirectory;

                if (firstItem != null)
                {
                    var path = firstItem.Path.Split('\\');
                    targetPath = Path.Combine(path.Take(path.Length - 1).ToArray());
                }
                else
                {
                    targetPath = CurrentDirectory;
                }
            }

            targetPath = (dropInfo.TargetItem as BaseDirectory)?.Path ?? targetPath;

            // targetItem.Add(sourceItem);
            var copyParameters = new CopyParameters
            {
                Source = (dropInfo.Data as BaseDirectory)?.Path,
                Target = targetPath
            };

            dialogService.ShowDialog("CopyDialog", new OverrideDialogParameters(copyParameters), r => { });
        }

        /// <summary>
        /// The create adorner layer.
        /// </summary>
        /// <param name="element">
        /// The element.
        /// </param>
        private void CreateAdornerLayer(UIElement element)
        {
            var listBox = element as ListBox;
            var ad = new AdornerDecorator();

            if (listBox?.Parent is Grid parent)
            {
                parent.Children.Remove(listBox);
                ad.Child = listBox;
                parent.Children.Add(ad);
            }
        }

        #endregion

        #region Initial the folder/file panel.

        /// <summary>
        /// The set columns.
        /// </summary>
        private void InitializeColumns()
        {
            AddFileColumns();
            AddFolderColumns();
            AddDriveColumns();
            AddPluginColumns();
        }

        /// <summary>
        /// Adds columns in the folder panel.
        /// </summary>
        private void AddFolderColumns()
        {
            new FolderColumnModel().GetColumn(
                (items, error) =>
                {
                    foreach (var column in items)
                    {
                        FolderPanelContainer.Columns.Add((GridViewColumn)column.Template);
                        ContextMenuBuild(column);
                    }
                });
        }

        /// <summary>
        /// Adds columns in the file panel.
        /// </summary>
        private void AddFileColumns()
        {
            new FileColumnModel().GetColumn((items, error) =>
            {
                foreach (var column in items)
                {
                    FilePanelContainer.Columns.Add((GridViewColumn)column.Template);
                    //this.ContextMenuBuild(column);
                }
            });
        }

        /// <summary>
        /// Adds columns for devices or drives.
        /// </summary>
        private void AddDriveColumns()
        {
            new DriveContainerModel().GetColumn(
                (columns, error) =>
                {
                    foreach (var column in columns)
                    {
                        DrivePanelContainer.Columns.Add((GridViewColumn)column.Template);
                    }
                });
        }

        #region Initial Plugin Columns

        /// <summary>
        /// Adds an additional columns that are provided by plugins.
        /// </summary>
        private void AddPluginColumns()
        {
            foreach (var pluginContext in pluginLoaderService.GetPluginContext())
            {
                foreach (var column in pluginContext.GetColumns())
                {
                    column.ColumnManager.SetUpdateCommand(UpdateColumnsCommand);
                    column.ColumnBuilder.UpdateColumnValue(column.ColumnManager);

                    if (InitialFolderColumnValues(column))
                    {
                        var columnNew = new GridViewColumn
                        {
                            Header = column.Header ?? "Error",
                            Width = column.Width,
                            DisplayMemberBinding = new Binding($"Additional[{column.Header}]")
                        };

                        FolderPanelContainer.Columns.Add(columnNew);
                    }

                    if (InitialFileColumnValues(column))
                    {
                        var columnNew = new GridViewColumn
                        {
                            Header = column.Header ?? "Error",
                            Width = column.Width,
                            CellTemplate = (DataTemplate)Application.Current.FindResource("ColumnTextDataTemplate")
                        };

                        FilePanelContainer.Columns.Add(columnNew);
                    }

                    PluginContextMenuBuild(column);
                }
            }
        }

        /// <summary>
        /// PluginContextMenuBuild
        /// </summary>
        private void PluginContextMenuBuild(IColumn column)
        {
            if (column.ContextItems == null) return;

            foreach (var item in column.ContextItems)
            {
                var menu = new MenuItem
                {
                    Header = item.Name
                };

                globalCommandService.SetCommand(new ()
                {
                    Command = item.Command,
                    ControlItem = menu,
                    XParamViewModel = new XParamViewModel(this, "CurrentDirectory")
                });

                ContextMenu.Items.Add(menu);
            }
        }

        /// <summary>
        /// ContextMenuBuild
        /// </summary>
        private void ContextMenuBuild(IColumn column)
        {
            if (column.ContextItems == null) return;

            foreach (var item in column.ContextItems)
            {
                ContextMenu.Items.Add(new MenuItem().SetParam(item.Name, item.CommandName, 
                    paramManager =>
                        {
                            paramManager.AddParam(this, "CurrentDirectory");
                            paramManager.AddParam(this, "CurrentDirectory");
                        }));
            }
        }

        /// <summary>
        /// Updates additional columns that are provided by plugins.
        /// </summary>
        private void UpdatePluginColumns()
        {
            foreach (var pluginContext in pluginLoaderService.GetPluginContext())
            {
                foreach (var column in pluginContext.GetColumns())
                {
                    //this.AddPluginColumns();
                    InitialFolderColumnValues(column);
                    InitialFileColumnValues(column);
                }
            }
        }

        /// <summary>
        /// Plugins uses this method to update columns.
        /// </summary>
        private void UpdateColumnsCommand()
        {
            foreach (var pluginContext in pluginLoaderService.GetPluginContext())
            {
                foreach (var column in pluginContext.GetColumns())
                {
                    foreach (var folder in DirectoryList)
                    {
                        var columnValue = column.ColumnBuilder.ColumnValueHandler(folder.Path);

                        if (columnValue != null)
                        {
                            folder.Additional[column.Header] = column.ColumnBuilder.ColumnValueHandler(folder.Path);
                        }
                    }

                    foreach (var file in FileList)
                    {
                        var columnValue = column.ColumnBuilder.ColumnValueHandler(file.Path);

                        if (columnValue != null)
                        {
                            file.Additional[column.Header] = column.ColumnBuilder.ColumnValueHandler(file.Path);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Initializes folder columns with values provided by plugins.
        /// </summary>
        /// <param name="column">
        /// Object implements contract of <see cref="IColumn"/>.
        /// </param>
        /// <returns>
        /// True if the ColumnValueHandler method returned all non-NULL values.
        /// <para> 
        /// The idea is to show the column where needed. In this way,
        /// you can independently implement the columns for folders and files.
        /// </para>
        /// </returns>
        private bool InitialFolderColumnValues(IColumn column)
        {
            var isAllEqNull = true;

            foreach (var folder in DirectoryList)
            {
                var columnValue = column.ColumnBuilder.ColumnValueHandler(folder.Path);

                if (!folder.Additional.ContainsKey(column.Header))
                {
                    folder.Additional.Add(column.Header, column.ColumnBuilder.ColumnValueHandler(folder.Path));
                    //folder.ContextItems = column.ContextItems;                   
                }
            }

            return isAllEqNull;
        }

        /// <summary>
        /// Initializes file columns with values provided by plugins.
        /// </summary>
        /// <param name="column">
        /// Object implements contract of <see cref="IColumn"/>.
        /// </param>
        /// <returns>
        /// True if the ColumnValueHandler method returned all non-NULL values.
        /// The idea is to show the column where needed. In this way,
        /// you can independently implement the columns for folders and files.
        /// </returns>
        private bool InitialFileColumnValues(IColumn column)
        {
            var isAllEqNull = true;

            foreach (var file in FileList)
            {
                var columnValue = column.ColumnBuilder.ColumnValueHandler(file.Path);

                if (!file.Additional.ContainsKey(column.Header))
                {
                    file.Additional.Add(column.Header, column.ColumnBuilder.ColumnValueHandler(file.Path));
                }
            }

            return isAllEqNull;
        }

        #endregion

        #endregion

        #region Helper Methods

        /// <summary>
        /// This method will try to restore the original state of the file panel 
        /// after the last time the program is closed, otherwise it will retry the database request.
        /// <see cref="SavePanelStateCommand"/>
        /// </summary>
        private void SetLastPanelState()
        {
            DirectoryPanelTemplate = (ControlTemplate)Application.Current.FindResource("DirectoryListViewTemplate");
            FileList = dataService.GetFiles(CurrentDirectory);
            DirectoryList = dataService.GetDirectories(CurrentDirectory);
            InitializeColumns();
        }

        /// <summary>
        /// Updates the file pane and sets the appropriate template.
        /// </summary>
        /// <param name="dirPath"> Expected the path to the directory. </param>
        private void UpdateFilePanel(object dirPath)
        {
            var template = (ControlTemplate)Application.Current.FindResource("DirectoryListViewTemplate");

            if (!DirectoryPanelTemplate.Equals(template))
            {
                DirectoryPanelTemplate = template;
            }

            var path = Directory.Exists(dirPath.ToString()) ? dirPath.ToString() : Directory.GetDirectoryRoot(dirPath.ToString());
            DirectoryList = dataService.GetDirectories(path);
            FileList = dataService.GetFiles(path);
            CurrentDirectory = path;
            UpdatePluginColumns();
        }

        /// <summary>
        /// Goes to the drive panel and sets the appropriate template..
        /// </summary>
        /// <param name="root">
        /// The root.
        /// </param>
        private void GoDrivePanel(object root)
        {
            DirectoryPanelTemplate = (ControlTemplate)Application.Current.FindResource("DriveListViewTemplate");
            DriveList = dataService.GetDrives();
            CurrentDirectory = (string)root;
        }

        /// <summary>
        /// Pre-registers commands to the command execution history.
        /// </summary>
        /// <param name="dirPath"> Expected the path to the directory. </param>
        private void SetCommands(string dirPath)
        {
            string[] paths = HelperFunctions.ParsePath(dirPath);

            navigationCommand.AddCommand(GoDrivePanel, "Root:C:\\");

            foreach (var path in paths)
            {
                navigationCommand.AddCommand(UpdateFilePanel, path);
            }
        }

        #endregion
    }
}