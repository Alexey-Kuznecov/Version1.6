// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SplitPanelViewModel.cs" company="T">
//   Copyright (p) Alexei Kuznecov. All right reserved.
// </copyright>
// <summary>
//   The left panel view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------


using UnityCommander.Common.Commands;
using UnityCommander.Integration.Commands;
using CommandNames = UnityCommander.Integration.Commands.CommandNames;

namespace UnityCommander.Modules.FilePanel.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;

    using Prism.Commands;
    using Prism.Regions;
    using Prism.Services.Dialogs;

    using UnityCommander.Common.Models.Directory;
    using UnityCommander.Common.Module;
    using UnityCommander.Core;
    using UnityCommander.Core.Commands;
    using UnityCommander.Core.DragDrop;
    using UnityCommander.Core.Helper;
    using UnityCommander.Core.Mvvm;
    using UnityCommander.Integration.Columns;
    using UnityCommander.Integration.Enums;
    using UnityCommander.Modules.FilePanel.Columns;
    using UnityCommander.Services.Interfaces;

    using CommandManager = UnityCommander.Core.Commands.CommandManager;

    /// <summary>
    /// The left panel view model.
    /// </summary>
    [Serializable]
    public partial class SplitPanelViewModel : RegionViewModelBase, IDropTarget, IDirectoryPanel
    {
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
        /// <param name="globalCommandService">
        /// The service for loading all detected plugin interfaces.
        /// </param>
        /// <param name="iconProvider">
        ///
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
            IPluginLoaderService pluginService,
            IGlobalCommandService globalCommandService,
            IIconProviderService iconProvider,
            IAppConfigService configService,
            CommandManager manager,
            ModuleLogger logger)
            : base(regionManager)
        {
            this.configService = configService;
            this.dialogService = dialogService;
            this.commandManager = manager;
            this.logger = logger.GetLogger();
            this.pluginLoaderService = pluginService;
            this.dataService = dataService;
            this.settingsService = settingsService.GetAppConfig();
            this.globalCommandService = globalCommandService;
            //this.TestCommand = this.globalCommandService.GetCommandManager().GetCommand("OnSettingsChanged").Command;
            //var fileManger = globalCommandService.GetCommandManager();
            //fileManger.CreateSingletonCommand(nameof(UpdateFilePanel), null, UpdateFilePanel);
            //fileManger.CreateCommand(this, GlobalCommandSelection.All);

            // Composite command
            this.multiCommandService = multiCommandService;
            this.multiCommandService.SaveCommand.RegisterCommand(this.SavePanelStateCommand);

            this.ThisComputerIcon = iconProvider.GetIcon(MaterialDesignThemes.Wpf.PackIconKind.LaptopWindows);
            this.BackButtonIcon = iconProvider.GetIcon(MaterialDesignThemes.Wpf.PackIconKind.ArrowBack);
            this.UpdateDirectoryIcon = iconProvider.GetIcon(MaterialDesignThemes.Wpf.PackIconKind.Refresh);
            this.ThisComputerIconIsEnabled = true;
            this.BackButtonIsEnabled = true;
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
        public DelegateCommand<BaseDirectory> SelectCurrentDirectoryItem => new DelegateCommand<BaseDirectory>(
            item =>
            {
                this.SelectedCurrentDirectoryItem = item;
            });

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

        #endregion

        #region Other

        /// <summary>
        /// Finalization of objects when unloading a module.
        /// </summary>
        public override void Destroy()
        {
            base.Destroy();

            // Detaching a command to avoid memory leaks.
            this.multiCommandService.SaveCommand.UnregisterCommand(this.SavePanelStateCommand);
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
                                 & dropInfo.TargetItem is ListBox or BaseDirectory;
            bool isSingleSelect = dropInfo.Data is BaseDirectory
                                  & dropInfo.TargetItem is ListBox or BaseDirectory;

            var adorner = AdornerLayer.GetAdornerLayer(dropInfo.VisualTarget);

            if (adorner == null)
                this.CreateAdornerLayer(dropInfo.VisualTarget);

            if (isMultiSelect || isSingleSelect)
                dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;

            if (dropInfo.Data is BaseDirectory & dropInfo.VisualTarget is ListBox & dropInfo.TargetItem == null)
                dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;

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
            var visualTarget = (dropInfo.VisualTarget as ListBox);
            var splitPanelViewModel = visualTarget.DataContext as SplitPanelViewModel;

            if (targetItem is null)
            {
                var firstItem = visualTarget?.SelectedItem as BaseDirectory;
                
                if (firstItem != null)
                {
                    var path = firstItem.Path.Split('\\');
                    targetPath = Path.Combine(path.Take(path.Length - 1).ToArray());
                }
                else
                    targetPath = this.CurrentDirectory;
            }

            this.dialogService.ShowDialog("CopyDialog", new OverrideDialogParameters(new CopyParameters
            {
                Source = (dropInfo.Data as BaseDirectory)?.Path,
                Target = (dropInfo.TargetItem as BaseDirectory)?.Path ?? targetPath
            }), r => { });

            
            //if (sourceItem is FolderModel folderModels)
            //    splitPanelViewModel.DirectoryList.Add(folderModels);
            //else
            //    splitPanelViewModel.FileList.Add(sourceItem as FileModel);
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

            if (listBox?.Parent is not Grid parent) return;
            parent.Children.Remove(listBox);
            ad.Child = listBox;
            parent.Children.Add(ad);
        }

        #endregion

        #region Initial the folder/file panel.

        /// <summary>
        /// The set columns.
        /// </summary>
        private void InitializeColumns()
        {
            this.AddFileColumns();
            this.AddFolderColumns();
            this.AddDriveColumns();
            this.AddPluginColumns();
            this.CreateContextMenu();
            this.navigationCommand = (NavigationInvoker)this.commandManager.GetCommand(this.Token);
            this.navigationCommand.OnExecuteChanged += this.OnExecuteChanged;
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
                        this.FolderPanelContainer.Columns.Add((GridViewColumn)column.Template);
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
                    this.FilePanelContainer.Columns.Add((GridViewColumn)column.Template);
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
                        this.DrivePanelContainer.Columns.Add((GridViewColumn)column.Template);
                    }
                });
        }

        #region Initial Plugin Columns

        /// <summary>
        /// Adds an additional columns that are provided by plugins.
        /// </summary>
        private void AddPluginColumns()
        {
            foreach (var pluginContext in this.pluginLoaderService.GetPluginContext())
            {
                foreach (var column in pluginContext.GetColumns())
                {
                    column.ColumnManager.SetUpdateCommand(this.UpdateColumnsCommand);
                    
                    column.ColumnBuilder.UpdateColumnValue(column.ColumnManager);

                    if (this.InitialFolderColumnValues(column))
                    {
                        var columnNew = new GridViewColumn
                        {
                            Header = column.Header ?? "Error",
                            Width = column.Width,
                            DisplayMemberBinding = new Binding($"Additional[{column.Header}]")
                        };

                        column.ColumnManager.SetHideCommand(this.HideColumnCommand, this.AddColumnCommand, columnNew);
                        this.FolderPanelContainer.Columns.Add(columnNew);
                    }

                    if (this.InitialFileColumnValues(column))
                    {
                        var columnNew = new GridViewColumn
                        {
                            Header = column.Header ?? "Error",
                            Width = column.Width,
                            CellTemplate = (DataTemplate)Application.Current.FindResource("ColumnTextDataTemplate")
                        };

                        this.FilePanelContainer.Columns.Add(columnNew);
                    }

                    this.CreatePluginContextMenu(column);
                }
            }
        }

        /// <summary>
        /// CreatePluginContextMenu
        /// </summary>
        private void CreatePluginContextMenu(IColumn column)
        {
            if (column.ContextItems == null) return;

            foreach (var item in column.ContextItems)
            {
                this.ContextMenu.Items.Add(
                    new MenuItem().SetParam(item.Command, paramManager =>
                            {
                                paramManager.AddParam(this, "CurrentDirectory");
                            }));
            }
        }

        /// <summary>
        /// CreateContextMenu
        /// </summary>
        private void CreateContextMenu()
        {
            var globalCommands = new List<GlobalCommand>
            {
                new () { DisplayName = CommandNames.ContentViewer, Name =  CommandNames.ContentViewer },
                new () { DisplayName = "Create", Name = CommandNames.FileCreate },
                new () { DisplayName = "Delete", Name = CommandNames.FileDelete },
                new () { DisplayName = "Move", Name = CommandNames.FileMove },
                new () { DisplayName = "Directory Update", Name = CommandNames.DirectoryUpdate },
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

        /// <summary>
        /// Updates additional columns that are provided by plugins.
        /// </summary>
        private void UpdatePluginColumns()
        {
            foreach (var pluginContext in this.pluginLoaderService.GetPluginContext())
            {
                foreach (var column in pluginContext.GetColumns())
                {
                    //this.AddPluginColumns();
                    this.InitialFolderColumnValues(column);
                    this.InitialFileColumnValues(column);
                }
            }
        }

        private void HideColumnCommand(DependencyObject column)
        {
            if (this.FolderPanelContainer.Columns.Contains(column))
            {
                this.FolderPanelContainer.Columns.Remove((GridViewColumn)column);
            }
        }

        private void AddColumnCommand(DependencyObject column)
        {
            if (!this.FolderPanelContainer.Columns.Contains(column))
            {
                this.FolderPanelContainer.Columns.Add((GridViewColumn)column);
            }
        }

        /// <summary>
        /// Plugins uses this method to update columns.
        /// </summary>
        private void UpdateColumnsCommand()
        {
            foreach (var pluginContext in this.pluginLoaderService.GetPluginContext())
            {
                foreach (var column in pluginContext.GetColumns())
                {
                    foreach (var folder in this.DirectoryList)
                    {
                        var columnValue = column.ColumnBuilder.ColumnValueHandler(column.Header, folder.Path, DirectoryItemType.Folder);

                        if (columnValue != null)
                        {
                            folder.Additional[column.Header] = column.ColumnBuilder.ColumnValueHandler(column.Header, folder.Path, DirectoryItemType.Folder);
                        }
                    }

                    foreach (var file in this.FileList)
                    {
                        var columnValue = column.ColumnBuilder.ColumnValueHandler(column.Header, file.Path, DirectoryItemType.File);

                        if (columnValue != null)
                        {
                            file.Additional[column.Header] = column.ColumnBuilder.ColumnValueHandler(column.Header, file.Path, DirectoryItemType.File);
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

            foreach (var folder in this.DirectoryList)
            {
                //var columnValue = column.ColumnBuilder.ColumnValueHandler(column.Header, folder.Path, DirectoryItemType.Folder);

                if (!folder.Additional.ContainsKey(column.Header))
                {
                    folder.Additional.Add(column.Header, column.ColumnBuilder.ColumnValueHandler(column.Header, folder.Path, DirectoryItemType.Folder));
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
            foreach (var file in this.FileList)
            {
                if (!file.Additional.ContainsKey(column.Header))
                    file.Additional.Add(column.Header, column.ColumnBuilder.ColumnValueHandler(column.Header, file.Path, DirectoryItemType.File));
            }

            return true;
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
            this.DirectoryPanelTemplate = (ControlTemplate)Application.Current.FindResource("DirectoryListViewTemplate");
            this.FileList = this.dataService.GetFiles(this.CurrentDirectory);
            this.DirectoryList = this.dataService.GetDirectories(this.CurrentDirectory);
            this.InitializeColumns();
        }

        /// <summary>
        /// Updates the file pane and sets the appropriate template.
        /// </summary>
        /// <param name="dirPath"> Expected the path to the directory. </param>
        private void UpdateFilePanel(object dirPath)
        {
            var template = (ControlTemplate)Application.Current.FindResource("DirectoryListViewTemplate");

            if (!this.DirectoryPanelTemplate.Equals(template))
                this.DirectoryPanelTemplate = template;

            var path = Directory.Exists(dirPath.ToString()) ? dirPath.ToString() : Directory.GetDirectoryRoot(dirPath.ToString());
            this.DirectoryList = this.dataService.GetDirectories(path);
            this.FileList = this.dataService.GetFiles(path);
            this.CurrentDirectory = path;
            this.UpdatePluginColumns();
        }

        public void DirectoryUpdate(IDirectoryPanel directoryPanel)
        {
            var template = (ControlTemplate)Application.Current.FindResource("DirectoryListViewTemplate");

            if (!this.DirectoryPanelTemplate.Equals(template))
                this.DirectoryPanelTemplate = template;

            var path = Directory.Exists(directoryPanel.GetCurrentPath()) 
                ? directoryPanel.GetCurrentPath() 
                : Directory.GetDirectoryRoot(directoryPanel.GetCurrentPath());
            this.DirectoryList = this.dataService.GetDirectories(path);
            this.FileList = this.dataService.GetFiles(path);
            this.CurrentDirectory = path;
            this.UpdatePluginColumns();
        }

        /// <summary>
        /// Goes to the drive panel and sets the appropriate template..
        /// </summary>
        /// <param name="root">
        /// The root.
        /// </param>
        private void GoDrivePanel(object root)
        {
            this.DirectoryPanelTemplate = (ControlTemplate)Application.Current.FindResource("DriveListViewTemplate");
            this.DriveList = this.dataService.GetDrives();
            this.CurrentDirectory = (string)root;
        }

        /// <summary>
        /// Pre-registers commands to the command execution history.
        /// </summary>
        /// <param name="dirPath"> Expected the path to the directory. </param>
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
    }
}