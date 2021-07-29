// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SplitPanelViewModel.cs" company="T">
//   Copyright (p) Alexey Kuznecov. All right reserved.
// </copyright>
// <summary>
//   The left panel view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Windows.Data;
using UnityCommander.Integration.Columns;

namespace UnityCommander.Modules.FilePanel.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Windows;
    using System.Windows.Controls;

    using Core.Helper;
    using Core.Mvvm;

    using GongSolutions.Wpf.DragDrop;
    
    using Prism.Commands;
    using Prism.Regions;

    using Services.Interfaces;

    using UnityCommander.Common.Models.Columns;
    using UnityCommander.Common.Models.Directory;
    using UnityCommander.Core;
    using UnityCommander.Core.Commands;
    using UnityCommander.Core.Modules;
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
        /// The navigationCommand class instance.
        /// </summary>
        private readonly NavigationInvoker navigationCommand;

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
        /// Control template for panel items.
        /// </summary>
        private ControlTemplate viewTemplate;

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
        ///  The service that provides the info of the file system items.
        /// </param>
        /// <param name="commandService">
        ///  The service that respond for composite commands.
        /// </param>
        /// <param name="pluginService">
        /// Service for loading all detected plugin interfaces.
        /// </param>
        /// <param name="manager">
        /// 
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
            var dds = regionManager.Regions;
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
            this.navigationCommand = (NavigationInvoker)manager.CommandRegister(new NavigationInvoker());
           
            this.DetectPanelOrientation();
            this.SetLastPanelState();
            {
                this.AddFileColumns();
                this.AddFolderColumns();
                this.AddDriveColumns();
            }
            this.SetAdditionalColumns();
            this.SetCommands(this.CurrentDirectory);
        }

        #endregion

        #region Commands

        /// <summary>
        /// Goes to the selected directory.
        /// </summary>
        public DelegateCommand<FolderModel> NavigateDirCommand => new DelegateCommand<FolderModel>(
            dir =>
        {
            if (dir != null)
            {
                this.navigationCommand.Execute(this.UpdateFilePanel, dir.Path);
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
                object[] saveObjects = { this.FileList, this.DirectoryList, this.CurrentDirectory };
                using FileStream fs = new FileStream(this.serializedPath, FileMode.OpenOrCreate);
                formatter.Serialize(fs, saveObjects);
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
        public ControlTemplate ViewTemplate 
        {
            get => this.viewTemplate;
            set => this.SetProperty(ref this.viewTemplate, value);
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
        public List<BaseDirectory> SelectedDirectories { get; set; } = new List<BaseDirectory>();

        /// <summary>
        /// 
        /// </summary>
        public Guid Token { get; set; } = Guid.NewGuid();

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
        /// The add columns in the folder panel.
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
        /// The add columns in the file panel.
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
        /// The add columns in the file panel.
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

        private void SetAdditionalColumns()
        {
            foreach (var pluginContext in pluginLoaderService.GetPluginContext())
            {
                foreach (var column in pluginContext.GetColumns())
                {
                    column.ColumnManager.SetUpdate(this.PluginUpdateColumns);
                    column.ColumnBuilder.UpdateColumnValue(column.ColumnManager);

                    if (this.InitialFolderColumnValues(column))
                    {
                       var columnNew = new GridViewColumn
                       {
                           Header = column.Header ?? "Error",
                           Width = column.Width,
                           DisplayMemberBinding = new Binding($"Additional[{column.Header}]")
                       };
                        
                        this.FolderPanelContainer.Columns.Add(columnNew);
                    }

                    if (this.InitialFileColumnValues(column))
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

        private void UpdateAdditionalColumns()
        {
            foreach (var pluginContext in pluginLoaderService.GetPluginContext())
            {
                foreach (var column in pluginContext.GetColumns())
                {
                    this.InitialFolderColumnValues(column);
                    this.InitialFileColumnValues(column);
                }
            }
        }

        private void PluginUpdateColumns()
        {
            foreach (var pluginContext in pluginLoaderService.GetPluginContext())
            {
                foreach (var column in pluginContext.GetColumns())
                {
                    foreach (var folder in this.DirectoryList)
                    {
                        var columnValue = column.ColumnBuilder.ColumnValueHandler(folder.Path);
                        
                        if (columnValue is not null)
                        {
                            folder.Additional[column.Header] = column.ColumnBuilder.ColumnValueHandler(folder.Path);
                        }
                    }

                    foreach (var file in this.FileList)
                    {
                        var columnValue = column.ColumnBuilder.ColumnValueHandler(file.Path);
                        
                        if (columnValue is not null)
                        {
                            file.Additional[column.Header] = column.ColumnBuilder.ColumnValueHandler(file.Path);
                        }
                    }
                }
            }
        }

        private bool InitialFolderColumnValues(IColumn column)
        {
            var isAllEqNull = false;

            foreach (var folder in this.DirectoryList)
            {
                var columnValue = column.ColumnBuilder.ColumnValueHandler(folder.Path);
                
                if (columnValue is not null)
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
        
        private bool InitialFileColumnValues(IColumn column)
        {
            var isAllEqNull = false;

            foreach (var file in this.FileList)
            {
                var columnValue = column.ColumnBuilder.ColumnValueHandler(file.Path);

                if (columnValue is not null)
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

        #region Helper Methods

        /// <summary>
        /// This method will try to restore the original state of the file panel 
        /// after the last time the program is closed, otherwise it will retry the database request.
        /// <see cref="SavePanelStateCommand"/>
        /// </summary>
        private void SetLastPanelState()
        {
            this.ViewTemplate = (ControlTemplate)Application.Current.FindResource("DirectoryListViewTemplate");

            if (settingsService.IsSessionSaved)
            {
                try
                {
                    BinaryFormatter formatter = new BinaryFormatter();

                    using (FileStream fs = new FileStream(this.serializedPath, FileMode.OpenOrCreate))
                    {
                        object[] deserializeState = (object[])formatter.Deserialize(fs);

                        this.FileList = deserializeState[0] as ObservableCollection<FileModel>;
                        this.DirectoryList = deserializeState[1] as ObservableCollection<FolderModel>;
                        this.CurrentDirectory = deserializeState[2] as string;
                    }

                    if (!Directory.Exists(this.CurrentDirectory))
                    {
                        this.CurrentDirectory = @"c:\";
                        throw new Exception();
                    }
                }
                catch (Exception)
                {
                    this.CurrentDirectory = @"D:\Works\WPF\UnityCommander\Version2.7.4";
                    this.FileList = this.dataProviderService.GetFiles(this.CurrentDirectory);
                    this.DirectoryList = this.dataProviderService.GetDirectories(this.CurrentDirectory);
                }
            }
            else
            {
                this.CurrentDirectory = @"D:\Works\WPF\UnityCommander\Version2.7.4";
                this.FileList = this.dataProviderService.GetFiles(this.CurrentDirectory);
                this.DirectoryList = this.dataProviderService.GetDirectories(this.CurrentDirectory);
            }
        }

        /// <summary>
        /// Goes to the directory panel.
        /// </summary>
        /// <param name="dirPath"> Expected the path to the directory. </param>
        private void UpdateFilePanel(object dirPath)
        {
            var path = Directory.Exists(dirPath.ToString()) ? (string)dirPath.ToString() : Directory.GetDirectoryRoot(dirPath.ToString());
            this.DirectoryList = this.dataProviderService.GetDirectories(path);
            this.FileList = this.dataProviderService.GetFiles(path);
            this.CurrentDirectory = path;
            this.UpdateAdditionalColumns();
        }

        /// <summary>
        /// Goes to the drive panel.
        /// </summary>
        private void GoDrivePanel()
        {
            this.ViewTemplate = (ControlTemplate)Application.Current.FindResource("DriveListViewTemplate");
            this.DriveList = this.dataProviderService.GetDrives();
        }

        /// <summary>
        /// Pre-registers commands to the command execution history.
        /// </summary>
        /// <param name="dirPath"> Expected the path to the directory. </param>
        private void SetCommands(string dirPath)
        {
            string[] paths = HelperFunctions.ParsePath(dirPath);

            this.navigationCommand.AddCommand(this.GoDrivePanel);

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