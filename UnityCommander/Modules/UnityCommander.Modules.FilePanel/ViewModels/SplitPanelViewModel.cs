// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LeftPanelViewModel.cs" company="T">
// Copyright (p) Alexey Kuznecov. All right reserved.  
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
    using System.Windows.Input;

    using Commands;
    using Common.Models;
    using Core.Helper;
    using Core.Mvvm;
    using GongSolutions.Wpf.DragDrop;
    using Prism.Commands;
    using Prism.Regions;
    using Services.Interfaces;

    using UnityCommander.Common.Models.Base;
    using UnityCommander.Common.Models.Columns;

    using Views;

    /// <summary>
    /// The left panel view model.
    /// </summary>
    [Serializable]
    public class SplitPanelViewModel : RegionViewModelBase, IDropTarget
    {
        #region Declaration fields

        /// <summary>
        /// Time field required only at the development stage.
        /// </summary>
        private static bool singleton;

        /// <summary>
        /// The copy dialog.
        /// </summary>
        private readonly CopyDialogView copyDialog;

        /// <summary>
        /// The invoker class instance.
        /// </summary>
        private readonly NavigationInvoker invoker;

        /// <summary>
        /// The directory provider.
        /// </summary>
        private readonly IDirectoryProvider directoryProviderManager;

        /// <summary>
        /// The file list.
        /// </summary>
        private ObservableCollection<FileModel> fileList;

        /// <summary>
        /// The directoryList.
        /// </summary>
        private ObservableCollection<FolderModel> directoryList;

        /// <summary>
        /// Indicates the current directory.
        /// </summary>
        private string currentDirectory;

        /// <summary>
        /// Time field required only at the development stage.
        /// </summary>
        private string dirPath;

        /// <summary>
        /// The common state service.
        /// </summary>
        private ICommonStateService commonStateService;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SplitPanelViewModel"/> class.
        /// </summary>
        /// <param name="regionManager">
        /// The region manager is Prism implementation.
        /// </param>
        /// <param name="directoryProvider">
        /// The service that provides the files system items.
        /// </param>
        /// <param name="commandService">
        /// The service that respond for composite commands.
        /// </param>
        public SplitPanelViewModel(IRegionManager regionManager, IDirectoryProvider directoryProvider, ICommonStateService commandService) 
            : base(regionManager)
        {

            this.commonStateService = commandService;
            this.commonStateService.SaveCommand.RegisterCommand(this.SaveStatePanelCommand);

            // Initialize properties.
            this.FilePanelContainer = new GridView();
            this.FolderPanelContainer = new GridView();
            this.copyDialog = new CopyDialogView();
            this.invoker = new NavigationInvoker();


            this.dirPath = @"c:\";
            this.directoryProviderManager = directoryProvider;
            this.SetLastStatePanel();
            this.AddColumnsFilePanel();
            this.AddColumnsFolderPanel();
            this.SetCommands(this.dirPath);
            this.CurrentDirectory = this.dirPath;
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
        /// The command name.
        /// </summary>
        public DelegateCommand SaveStatePanelCommand => new DelegateCommand(
            () =>
        {
            BinaryFormatter formatter = new BinaryFormatter();

            object[] saveObjects = { this.FileList, this.DirectoryList };

            using (FileStream fs = new FileStream("filepanel.dat", FileMode.OpenOrCreate))
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
        /// The destroy.
        /// </summary>
        public override void Destroy()
        {
            base.Destroy();
            this.commonStateService.SaveCommand.UnregisterCommand(this.SaveStatePanelCommand);
        }

        /// <summary>
        /// The set last state panel.
        /// </summary>
        public void SetLastStatePanel()
        {
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();

                using (FileStream fs = new FileStream("filepanel.dat", FileMode.OpenOrCreate))
                {
                    object[] deserializeState = (object[])formatter.Deserialize(fs);

                    this.FileList = deserializeState[0] as ObservableCollection<FileModel>;
                    this.DirectoryList = deserializeState[1] as ObservableCollection<FolderModel>;
                }
            }
            catch (Exception e)
            {
                if (Directory.Exists(this.dirPath))
                {
                    this.FileList = this.directoryProviderManager.GetFiles(this.dirPath);
                    this.DirectoryList = this.directoryProviderManager.GetDirectories(this.dirPath);
                }
            }
        }

        #region Helper Methods

        /// <summary>
        /// The add columns in the file panel.
        /// </summary>
        private void AddColumnsFilePanel()
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
        private void AddColumnsFolderPanel()
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
        /// The navigate directory.
        /// </summary>
        /// <param name="dirPath"> Expected the path to the directory. </param>
        private void UpdateFilePanel(object dirPath)
        {
            string path = (string)dirPath;
            this.DirectoryList = this.directoryProviderManager.GetDirectories(path);
            this.FileList = this.directoryProviderManager.GetFiles(path);
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