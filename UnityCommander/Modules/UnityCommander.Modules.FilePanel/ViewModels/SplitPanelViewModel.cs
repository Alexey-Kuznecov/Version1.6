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
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Windows;
    using System.Windows.Controls;

    using Commands;

    using Core.Helper;
    using Core.Mvvm;

    using GongSolutions.Wpf.DragDrop;

    using Prism.Commands;
    using Prism.Regions;

    using Services.Interfaces;

    using UnityCommander.Common.Models.Columns;
    using UnityCommander.Common.Models.Directory;
    using UnityCommander.Integration.Models.Base;
    using UnityCommander.Services;

    using Views;

    /// <summary>
    /// The left panel view model.
    /// </summary>
    [Serializable]
    public class SplitPanelViewModel : RegionViewModelBase, IDropTarget
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
        /// The invoker class instance.
        /// </summary>
        private readonly NavigationInvoker invoker;

        #region Dependencies Injection

        /// <summary>
        /// The directory provider.
        /// </summary>
        private readonly IDirectoryProviderService directoryProviderService;

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
        /// The directoryList.
        /// </summary>
        private ObservableCollection<FolderModel> directoryList;

        #endregion

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
        /// <param name="directoryProviderService">
        ///  The service that provides the files system items.
        /// </param>
        /// <param name="commandService">
        ///  The service that respond for composite commands.
        /// </param>
        /// <param name="pluginLoaderService">
        /// Service for loading all detected plugin interfaces.
        /// </param>
        public SplitPanelViewModel(
            IRegionManager regionManager, 
            ISettingsProviderService settingsService, 
            IDirectoryProviderService directoryProviderService, 
            IGlobalCommandService commandService, 
            IPluginLoaderService pluginLoaderService) 
            : base(regionManager)
        {
            this.directoryProviderService = directoryProviderService;
            this.settingsService = settingsService.GetAppConfig();
            this.pluginLoaderService = pluginLoaderService;

            // Composite command
            this.globalCommandService = commandService;
            this.globalCommandService.SaveCommand.RegisterCommand(this.SavePanelStateCommand);

            this.pluginLoaderService.SetPluginDependencies();

            // Initialize properties.
            this.FilePanelContainer = new GridView();
            this.FolderPanelContainer = new GridView();
            this.copyDialog = new CopyDialogView();
            this.invoker = new NavigationInvoker();

            this.DetectPanelOrientation();
            this.SetLastPanelState();
            this.AddFileColumns();
            this.AddFolderColumns();
            this.SetAdditionalColumns();
            this.GetFileColumnValues();
            this.SetCommands(this.CurrentDirectory);
        }

        #endregion

        #region Commands

        /// <summary>
        /// Goes to the selected directory.
        /// </summary>
        public DelegateCommand<FolderModel> GoToDirectory => new DelegateCommand<FolderModel>(
            dir =>
        {
            if (dir != null)
            {
                this.invoker.Execute(this.UpdateFilePanel, dir.Path);
            }
        });

        /// <summary>
        ///  Goes back to the previous directory.
        /// </summary>
        public DelegateCommand GoBackDirectory => new DelegateCommand(() =>
        {
            if (this.invoker.CanUndo)
            {
                this.invoker.Previous();
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
                this.invoker.Execute(this.UpdateFilePanel, dir);
            }
        });

        /// <summary>
        /// The command serializes the state of the file panel after the program is closed
        /// to restore it to its original state the next time it starts. <see cref="SetLastPanelState"/>
        /// </summary>
        public DelegateCommand SavePanelStateCommand => new DelegateCommand(
            () =>
        {
            BinaryFormatter formatter = new BinaryFormatter();

            object[] saveObjects = { this.FileList, this.DirectoryList, this.CurrentDirectory };

            using (FileStream fs = new FileStream(this.serializedPath, FileMode.OpenOrCreate))
            {
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

        /// <summary>
        /// This method will try to restore the original state of the file panel 
        /// after the last time the program is closed, otherwise it will retry the database request.
        /// <see cref="SavePanelStateCommand"/>
        /// </summary>
        private void SetLastPanelState()
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
                }
            }
            catch (Exception e)
            {
                this.CurrentDirectory = @"D:\Works\WPF\UnityCommander\Version2.7.4";
                this.FileList = this.directoryProviderService.GetFiles(this.CurrentDirectory);
                this.DirectoryList = this.directoryProviderService.GetDirectories(this.CurrentDirectory);
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

        #region Helper Methods

        /// <summary>
        /// The add columns in the file panel.
        /// </summary>
        private void AddFileColumns()
        {
            FileColumnModel colsDefault = new FileColumnModel();

            // Forced addition columns to the directory panel.
            colsDefault.GetColumn((items, error) =>
            {
                foreach (var column in items)
                {
                    this.FilePanelContainer.Columns.Add((GridViewColumn)column.Template);
                }
            });
        }

        /// <summary>
        /// The add columns in the folder panel.
        /// </summary>
        private void AddFolderColumns()
        {
            FolderColumnModel colsDefault = new FolderColumnModel();

            // Forced addition columns to the directory panel.
            colsDefault.GetColumn((items, error) =>
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
        private void SetAdditionalColumns()
        {
            foreach (var column in pluginLoaderService.GetPluginImplements().GetColumns(this.pluginLoaderService))
            {
                // Force columns to be added to the catalog pane.
                if (column.IsDisplayed)
                {
                    if (column.TargetPanel == Integration.Enums.TargetPanel.Files)
                    {
                        this.FilePanelContainer.Columns.Add((GridViewColumn)column.Template);
                    }
                    else
                    {
                        this.FolderPanelContainer.Columns.Add((GridViewColumn)column.Template);
                    }
                }
            }
        }

        /// <summary>
        /// The get column value.
        /// </summary>
        private void GetFileColumnValues()
        {
            var models = new ObservableCollection<FileModel>();

            foreach (var file in this.FileList)
            {
                try
                {
                    pluginLoaderService.GetContent(
                        (import) => models.Add(file.MergeObjectProperties<FileModel>(import.GetColumnValues(file.Path))));
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                    throw;
                }

                this.FileList = models;
                ExtensionMethod.StoredReset();
            }
        }

        /// <summary>
        /// The navigate directory.
        /// </summary>
        /// <param name="dirPath"> Expected the path to the directory. </param>
        private void UpdateFilePanel(object dirPath)
        {
            string path = (string)dirPath;
            this.DirectoryList = this.directoryProviderService.GetDirectories(path);
            this.FileList = this.directoryProviderService.GetFiles(path);
            this.CurrentDirectory = path;
        }

        /// <summary>
        /// Pre-registers commands to the command execution history.
        /// </summary>
        /// <param name="dirPath"> Expected the path to the directory. </param>
        private void SetCommands(string dirPath)
        {
            string[] paths = HelperFunctions.ParsePath(dirPath);

            foreach (var item in paths)
            {
                this.invoker.AddCommand(this.UpdateFilePanel, item);
            }
        }

        #endregion
    }
}