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
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.IO;
    using System.Windows;
    using System.Windows.Input;

    using GongSolutions.Wpf.DragDrop;

    using Prism.Commands;
    using Prism.Regions;

    using UnityCommander.Common;
    using UnityCommander.Common.Models;
    using UnityCommander.Core.Mvvm;
    using UnityCommander.Integration.Contracts;
    using UnityCommander.Modules.FilePanel.Commands;
    using UnityCommander.Modules.FilePanel.Views;
    using UnityCommander.Services.Interfaces;

    /// <summary>
    /// The left panel view model.
    /// </summary>
    public class LeftPanelViewModel : RegionViewModelBase, IDropTarget
    {
        #region Declaration fields

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
        /// Previous a directory dirPath.
        /// </summary>
        private string previousDirectory;

        /// <summary>
        /// The file list.
        /// </summary>
        private ObservableCollection<FileModel> fileList;

        /// <summary>
        /// The directoryList.
        /// </summary>
        private ObservableCollection<DirectoryModel> directoryList;

        /// <summary>
        /// The navigator.
        /// </summary>
        private INavigator navigator;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LeftPanelViewModel"/> class.
        /// </summary>
        /// <param name="regionManager">
        /// The region manager is Prism implementation.
        /// </param>
        /// <param name="directoryProvider">
        /// The service that provides the files system items.
        /// </param>
        public LeftPanelViewModel(IRegionManager regionManager, IDirectoryProvider directoryProvider) 
            : base(regionManager)
        {
            string rightPanelPath = @"Works\UnitTests\";

            this.invoker = new NavigationInvoker();
            string path = string.Empty;
            
            this.directoryProviderManager = directoryProvider;

            this.copyDialog = new CopyDialogView();
            string[] drives = Environment.GetLogicalDrives();
            foreach (string drive in drives)
            {
                path = drive + rightPanelPath;

                if (Directory.Exists(path))
                {
                    this.FileList = this.directoryProviderManager.GetFiles(path);
                    this.DirectoryList = this.directoryProviderManager.GetDirectories(path);
                }
            }

            this.SetCommands(path);
        }

        #endregion

        #region Commands

        /// <summary>
        /// The go to directory.
        /// </summary>
        public DelegateCommand<DirectoryModel> GotoDirectory => new DelegateCommand<DirectoryModel>(dir =>
        {
            if (dir != null)
            {
                this.invoker.Execute(NavigateCommand, dir.Path);
            }
        });

        /// <summary>
        /// The go to directory.
        /// </summary>
        public ICommand PrevDirectory => new DelegateCommand<DirectoryModel>(dir =>
        {
            if (this.invoker.CanUndo)
            {
                this.invoker.Previous();
            }
        });

        #endregion Commands End

        #region Delaration properties

        /// <summary>
        /// Gets or sets the UI collection.
        /// </summary>
        public INavigator Navigator
        {
            get => this.navigator;
            set
            {
                this.previousDirectory = value.PreviousDirectory;
                this.SetProperty(ref this.navigator, value);
            }
        }

        /// <summary>
        /// Gets or sets the directory list.
        /// </summary>
        public ObservableCollection<DirectoryModel> DirectoryList
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

        #endregion

        /// <summary>
        /// The drag over.
        /// </summary>
        /// <param name="dropInfo">
        /// The drop-over event handler.
        /// </param>
        [DebuggerStepThrough]
        void IDropTarget.DragOver(IDropInfo dropInfo)
        {
            if (dropInfo.Data is DirectoryBase source && dropInfo.TargetItem is DirectoryBase target)
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
        [DebuggerStepThrough]
        void IDropTarget.Drop(IDropInfo dropInfo)
        {
            DirectoryBase sourceItem = dropInfo.Data as DirectoryBase;
            DirectoryBase targetItem = dropInfo.TargetItem as DirectoryBase;
            
            // targetItem.Add(sourceItem);
            if (this.copyDialog.DataContext is CopyDialogViewModel dialogViewModel)
            {
                dialogViewModel.Source = (dropInfo.Data as DirectoryBase)?.Path;
                dialogViewModel.Target = (dropInfo.TargetItem as DirectoryBase)?.Path;
            }

            this.copyDialog.ShowDialog();
        }

        /// <summary>
        /// The navigate directory.
        /// </summary>
        /// <param name="dirPath"> Expected the path to the directory. </param>
        private void NavigateCommand(object dirPath)
        {
            this.DirectoryList = this.directoryProviderManager.GetDirectories((string)dirPath);
            this.FileList = this.directoryProviderManager.GetFiles((string)dirPath);
        }

        /// <summary>
        /// Pre-registers commands to the command execution history.
        /// </summary>
        /// <param name="dirPath"> Expected the path to the directory. </param>
        private void SetCommands(string dirPath)
        {
            string[] paths = HelperMethods.ParsePath(dirPath);

            foreach (var item in paths)
            {
                this.invoker.AddCommand(this.NavigateCommand, item);
            }
        }
    }
}
