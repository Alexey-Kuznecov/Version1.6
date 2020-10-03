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
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    using GongSolutions.Wpf.DragDrop;

    using Prism.Commands;
    using Prism.Regions;
    using UnityCommander.Common.Models;
    using UnityCommander.Core.IO;
    using UnityCommander.Core.Mvvm;
    using UnityCommander.Integration.Contracts;
    using UnityCommander.Modules.FilePanel.Views;
    using UnityCommander.Services.Interfaces;

    /// <summary>
    /// The left panel view model.
    /// </summary>
    [SuppressMessage("ReSharper", "StyleCop.SA1503")]
    public class LeftPanelViewModel : RegionViewModelBase, IDropTarget
    {
        /// <summary>
        /// The directory provider.
        /// </summary>
        private static IDirectoryProvider directoryProviderManager;

        /// <summary>
        /// The copy dialog.
        /// </summary>
        private readonly CopyDialogView copyDialog;

        /// <summary>
        /// The invoker.
        /// </summary>
        private readonly NavigationInvoker invoker;

        /// <summary>
        /// The file list.
        /// </summary>
        private ObservableCollection<FileModel> fileList;

        /// <summary>
        /// The directoryList.
        /// </summary>
        private ObservableCollection<DirectoryModel> directoryList;

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
            directoryProviderManager = directoryProvider;

            this.copyDialog = new CopyDialogView();
            string rightPanelPath = @"Works\UnitTests\";

            string[] drives = Environment.GetLogicalDrives();
            foreach (string drive in drives)
            {
                string path = drive + rightPanelPath;

                if (Directory.Exists(path))
                {
                    this.FileList = directoryProviderManager.GetFiles(path);
                    this.DirectoryList = directoryProviderManager.GetDirectories(path);
                }
            }

            this.invoker = new NavigationInvoker();
        }

        /// <summary>
        /// The go to directory.
        /// </summary>
        public DelegateCommand<DirectoryModel> GotoDirectory => new DelegateCommand<DirectoryModel>(dir =>
        {
            if (dir == null) return;
            this.DirectoryList = directoryProviderManager.GetDirectories(dir.Path);
            this.FileList = directoryProviderManager.GetFiles(dir.Path);
            
            invoker.Display(dir.Path);
        });

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

        /// <summary>
        /// The drag over.
        /// </summary>
        /// <param name="dropInfo">
        /// The drop-over event handler.
        /// </param>
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
    }
}
