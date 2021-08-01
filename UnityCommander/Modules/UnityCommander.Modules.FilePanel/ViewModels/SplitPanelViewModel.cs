// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SplitPanelViewModel.cs" company="T">
//   Copyright (p) Alexey Kuznecov. All right reserved.
// </copyright>
// <summary>
//   The left panel view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace UnityCommander.Modules.FilePanel.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;

    using Core.Helper;
    using Core.Mvvm;

    using GongSolutions.Wpf.DragDrop;

    using Prism.Commands;
    using Prism.Regions;

    using Services.Interfaces;

    using UnityCommander.Common.Models.Columns;
    using UnityCommander.Common.Models.Directory;
    using UnityCommander.Core.Commands;
    using UnityCommander.Core.Modules;
    using UnityCommander.Integration.Columns;

    using Views;

    /// <summary>
    /// The left panel view model.
    /// </summary>
    [Serializable]
    public class SplitPanelViewModel : RegionViewModelBase, IDropTarget, IDirectoryPanel
    {
        #region Declaration fields

        /// <summary>
        /// Indicates the instance number used to determine the orientation of the file panel.   
        /// </summary>
        private static byte numberInstances;

        /// <summary>
        /// The copy dialog.
        /// </summary>
        private readonly CopyDialogView copyDialog;

        /// <summary>
        /// If true, the plugin was cached and the result will be restored
        /// from the cache table the next time the program starts.
        /// </summary>
        private bool pluginValuesIsCached;

        #region Dependencies Injection

        /// <summary>
        /// The directory provider.
        /// </summary>
        private readonly IDataProviderService dataProviderService;

        /// <summary>
        /// The application settings.
        /// </summary>
        private readonly ISettings settingsService;

        /// <summary>
        /// The common state service.
        /// </summary>
        private readonly IGlobalCommandService globalCommandService;

        /// <summary>
        /// The plugin loader service.
        /// </summary>
        private readonly IPluginLoaderService pluginLoaderService;

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
        /// The command manager.
        /// </summary>
        private CommandManager commandManager;

        /// <summary>
        /// Control template for panel items.
        /// </summary>
        private ControlTemplate directoryPanelTemplate;

        /// <summary>
        /// Serialization path for saving the file panel state.
        /// </summary>
        private string serializedPath;

        /// <summary>
        /// Indicates the current directory.
        /// </summary>
        private string currentDirectory;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SplitPanelViewModel"/> class.
        /// </summary>
        /// <param name="regionManager">
        /// The region manager is Prism implementation.
        /// </param>
        /// <param name="settingsService">
        /// Gets interface to configure of application.
        /// </param>
        /// <param name="dataProviderService">
        /// The service that provides the info of the file system items.
        /// </param>
        /// <param name="commandService">
        /// The service that respond for composite commands.
        /// </param>
        /// <param name="pluginService">
        /// Service for loading all detected plugin interfaces.
        /// </param>
        /// <param name="manager">
        /// Command manager
        /// </param>
        public SplitPanelViewModel(
            IRegionManager regionManager, 
            ISettingsProviderService settingsService, 
            IDataProviderService dataProviderService, 
            IGlobalCommandService commandService,
            IPluginLoaderService pluginService, 
            CommandManager manager)
            : base(regionManager)
        {
            this.commandManager = manager;
            this.pluginLoaderService = pluginService;
            this.dataProviderService = dataProviderService;
            this.settingsService = settingsService.GetAppConfig();

            // Composite command
            this.globalCommandService = commandService;
            this.globalCommandService.SaveCommand.RegisterCommand(this.SavePanelStateCommand);

            // Initialize properties.
            this.FilePanelContainer = new ();
            this.FolderPanelContainer = new ();
            this.DrivePanelContainer = new ();
            this.copyDialog = new ();
           
            this.DetectPanelOrientation();
            this.SetLastPanelState();
        }

        #endregion

        #region Commands

        /// <summary>
        /// Goes to the selected directory.
        /// </summary>
        public DelegateCommand<FolderModel> NavigateDirectoryCommand => new DelegateCommand<FolderModel>(
            dir =>
        {
            if (dir != null)
            {
                this.navigationCommand.Execute(this.UpdateFilePanel, dir.Path);
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
                this.navigationCommand.Execute(this.UpdateFilePanel, dir.Letter);
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
                this.navigationCommand.Execute(this.UpdateFilePanel, dir);
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
                BinaryFormatter formatter = new BinaryFormatter();

                foreach (var file in this.FileList)
                {
                    file.Additional = file.Additional.ConvertToDictionary();
                }

                foreach (var folder in this.DirectoryList)
                {
                    folder.Additional = folder.Additional.ConvertToDictionary();
                }

                object[] saveObjects = { this.FileList, this.DirectoryList, this.CurrentDirectory };
                
                using (FileStream fs = new FileStream(this.serializedPath, FileMode.OpenOrCreate))
                {
                    formatter.Serialize(fs, saveObjects);
                }
            }
        });

        #endregion

        #region Delaration properties

        /// <summary>
        /// Gets or sets the current directory.
        /// </summary>
        public string CurrentDirectory
        {
            get => this.currentDirectory;
            set => this.SetProperty(ref this.currentDirectory, value);
        }

        /// <summary>
        /// Gets or sets grid view for file panel.
        /// </summary>
        public GridView FilePanelContainer { get; set; }

        /// <summary>
        /// Gets or sets grid view for folder panel.
        /// </summary>
        public GridView FolderPanelContainer { get; set; }

        /// <summary>
        /// Gets or sets grid view for folder panel.
        /// </summary>
        public GridView DrivePanelContainer { get; set; }

        #region Collection Data

        /// <summary>
        /// Gets or sets the directory list.
        /// </summary>
        public ObservableCollection<FolderModel> DirectoryList
        {
            get => this.directoryList;
            set => this.SetProperty(ref this.directoryList, value);
        }

        /// <summary>
        /// Gets or sets the file list.
        /// </summary>
        public ObservableCollection<FileModel> FileList
        {
            get => this.fileList;
            set => this.SetProperty(ref this.fileList, value);
        }

        /// <summary>
        /// Gets or sets the file list.
        /// </summary>
        public ObservableCollection<DriveModel> DriveList
        {
            get => this.driveList;
            set => this.SetProperty(ref this.driveList, value);
        }

        #endregion

        /// <summary>
        /// Gets or sets the template for panel items.
        /// </summary>
        public ControlTemplate DirectoryPanelTemplate 
        {
            get => this.directoryPanelTemplate;
            set => this.SetProperty(ref this.directoryPanelTemplate, value);
        }

        /// <summary>
        /// Sets the selected directory.
        /// </summary>
        public BaseDirectory SelectedBaseDirectory
        {
            set
            {
                if (value == null) return;
                this.SelectedDirectories.Add(value);
            }
        }

        /// <summary>
        /// Gets or sets the selected directory.
        /// </summary>
        public List<BaseDirectory> SelectedDirectories { get; set; } = new ();

        #region ADDITIONAL INITIALIZATION VIEW MODEL

        /// <summary>
        /// Gets or sets the token.
        /// </summary>
        public Guid Token { get; set; }

        /// <summary>
        /// Gets or sets the instance number.
        /// </summary>
        public byte InstanceNumber { get; set; }

        /// <summary>
        /// Gets or sets the token.
        /// </summary>
        public static byte InstanceCount { get; set; }

        /// <summary>
        /// The initial panel.
        /// </summary>
        public void InitializedViewModel()
        {
            this.InstanceNumber = ++InstanceCount;
            this.Token = this.Token == Guid.Empty ? Guid.NewGuid() : this.Token;
            this.navigationCommand = (NavigationInvoker)this.commandManager.CommandRegister(this.Token, new NavigationInvoker());
            this.SetCommands(this.CurrentDirectory);
        }

        /// <summary>
        /// The initial panel.
        /// </summary>
        /// <returns>
        /// The <see cref="byte"/>.
        /// </returns>
        public byte GetInstanceNumber()
        {
            return this.InstanceNumber;
        }

        /// <summary>
        /// The get panel token.
        /// </summary>
        /// <returns>
        /// The <see cref="Guid"/>.
        /// </returns>
        public Guid GetPanelToken()
        {
            if (this.Token == Guid.Empty)
            {
                this.Token = Guid.NewGuid();
                return this.Token;
            }

            return this.Token;
        }

        #endregion

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


            if (isMultiSelect || isSingleSelect) 
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                dropInfo.Effects = DragDropEffects.Copy;
            }
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
            
            // targetItem.Add(sourceItem);
            if (this.copyDialog.DataContext is CopyDialogViewModel dialogViewModel)
            {
                dialogViewModel.Source = (dropInfo.Data as BaseDirectory)?.Path;
                dialogViewModel.Target = (dropInfo.TargetItem as BaseDirectory)?.Path;
            }

            this.copyDialog.ShowDialog();
        }

        #endregion

        /// <summary>
        /// Finalization of objects when unloading a module.
        /// </summary>
        public override void Destroy()
        {
            base.Destroy();

            // Detaching a command to avoid memory leaks.
            this.globalCommandService.SaveCommand.UnregisterCommand(this.SavePanelStateCommand);
        }

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

                    if (this.InitialFolderColumnValues(column) || this.pluginValuesIsCached)
                    {
                        var columnNew = new GridViewColumn
                        {
                            Header = column.Header ?? "Error",
                            Width = column.Width,
                            DisplayMemberBinding = new Binding($"Additional[{column.Header}]")
                        };

                        this.FolderPanelContainer.Columns.Add(columnNew);
                    }

                    if (this.InitialFileColumnValues(column) || this.pluginValuesIsCached)
                    {
                        var columnNew = new GridViewColumn
                        {
                            Header = column.Header ?? "Error",
                            Width = column.Width,
                            DisplayMemberBinding = new Binding($"Additional[{column.Header}]")
                        };

                        this.FilePanelContainer.Columns.Add(columnNew);
                    }
                }
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
                    this.InitialFolderColumnValues(column);
                    this.InitialFileColumnValues(column);
                }
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
                        var columnValue = column.ColumnBuilder.ColumnValueHandler(folder.Path);

                        if (columnValue != null)
                        {
                            folder.Additional[column.Header] = column.ColumnBuilder.ColumnValueHandler(folder.Path);
                        }
                    }

                    foreach (var file in this.FileList)
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
        /// The idea is to show the column where needed. In this way,
        /// you can independently implement the columns for folders and files.
        /// </returns>
        private bool InitialFolderColumnValues(IColumn column)
        {
            var isAllEqNull = false;

            foreach (var folder in this.DirectoryList)
            {
                var columnValue = column.ColumnBuilder.ColumnValueHandler(folder.Path);

                if (columnValue != null)
                {
                    if (!folder.Additional.ContainsKey(column.Header))
                    {
                        folder.Additional.Add(column.Header, column.ColumnBuilder.ColumnValueHandler(folder.Path));
                        isAllEqNull = true;
                    }
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
            var isAllEqNull = false;

            foreach (var file in this.FileList)
            {
                var columnValue = column.ColumnBuilder.ColumnValueHandler(file.Path);

                if (columnValue != null)
                {
                    if (!file.Additional.ContainsKey(column.Header))
                    {
                        file.Additional.Add(column.Header, column.ColumnBuilder.ColumnValueHandler(file.Path));
                        isAllEqNull = true;
                    }
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
            this.DirectoryPanelTemplate = (ControlTemplate)Application.Current.FindResource("DirectoryListViewTemplate");
            
            try
            {
                if (this.settingsService.IsSessionSaved)
                {
                    BinaryFormatter formatter = new BinaryFormatter();

                    using (FileStream fs = new FileStream(this.serializedPath, FileMode.OpenOrCreate))
                    {
                        object[] deserializeState = (object[])formatter.Deserialize(fs);

                        this.FileList = (ObservableCollection<FileModel>)deserializeState[0];
                        this.DirectoryList = (ObservableCollection<FolderModel>)deserializeState[1];

                        this.CurrentDirectory = deserializeState[2] as string;
                    }


                    this.pluginValuesIsCached = true;
                    this.InitializeColumns();

                    if (!Directory.Exists(this.CurrentDirectory))
                    {
                        throw new Exception();
                    }
                }
                else
                {
                    throw new Exception();
                }
            } 
            catch
            {
                this.CurrentDirectory = @"C:\";
                this.FileList = this.dataProviderService.GetFiles(this.CurrentDirectory);
                this.DirectoryList = this.dataProviderService.GetDirectories(this.CurrentDirectory);
                this.InitializeColumns();
            }
        }



        /// <summary>
        /// Updates the file pane and sets the appropriate template.
        /// </summary>
        /// <param name="dirPath"> Expected the path to the directory. </param>
        private void UpdateFilePanel(object dirPath)
        {
            var template = (ControlTemplate)Application.Current.FindResource("DirectoryListViewTemplate");

            if (!this.DirectoryPanelTemplate.Equals(template))
            {
                this.DirectoryPanelTemplate = template;
            }
            
            var path = Directory.Exists(dirPath.ToString()) ? (string)dirPath.ToString() : Directory.GetDirectoryRoot(dirPath.ToString());
            this.DirectoryList = this.dataProviderService.GetDirectories(path);
            this.FileList = this.dataProviderService.GetFiles(path);
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
            this.DriveList = this.dataProviderService.GetDrives();
            this.CurrentDirectory = (string)root;
        }

        /// <summary>
        /// Pre-registers commands to the command execution history.
        /// </summary>
        /// <param name="dirPath"> Expected the path to the directory. </param>
        private void SetCommands(string dirPath)
        {
            string[] paths = HelperFunctions.ParsePath(dirPath);

            this.navigationCommand.AddCommand(this.GoDrivePanel, "Root:C:\\");

            foreach (var path in paths)
            {
                this.navigationCommand.AddCommand(this.UpdateFilePanel, path);
            }
        }

        /// <summary>
        /// Identifies the panel and sets the appropriate path for serialization to the file panel state.
        /// TODO: Now this is a temporary solution, in the future there should be rework.
        /// </summary>
        private void DetectPanelOrientation()
        {
            string[] session = this.settingsService.SessionFiles.Split(',');
            this.serializedPath = numberInstances == 0 ? session[0] : session[1];
            numberInstances++;
        }

        #endregion
    }
}