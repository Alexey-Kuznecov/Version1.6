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
    using System.IO;
    using System.Windows;

    using GongSolutions.Wpf.DragDrop;

    using Prism.Regions;

    using UnityCommander.Business;
    using UnityCommander.Core.Mvvm;
    using UnityCommander.Modules.FilePanel.Views;
    using UnityCommander.Services.Interfaces;

    /// <summary>
    /// The left panel view model.
    /// </summary>
    public class LeftPanelViewModel : RegionViewModelBase, IDropTarget
    {
        /// <summary>
        /// The copy dialog.
        /// </summary>
        private readonly CopyDialogView copyDialog;

        /// <summary>
        /// The file list.
        /// </summary>
        private ObservableCollection<FileModel> fileList;

        /// <summary>
        /// Initializes a new instance of the <see cref="LeftPanelViewModel"/> class.
        /// </summary>
        /// <param name="regionManager">
        /// The region Manager.
        /// </param>
        /// <param name="filesProvider">
        /// The files Provider.
        /// </param>
        public LeftPanelViewModel(IRegionManager regionManager, IFilesProvider filesProvider) :
            base(regionManager)
        {
            this.copyDialog = new CopyDialogView();
            string rightPanelPath = @"Works\UnitTests\";

            string[] drives = Environment.GetLogicalDrives();
            foreach (string drive in drives)
            {
                string path = drive + rightPanelPath;

                if (Directory.Exists(path))
                {
                    this.FileList = filesProvider.GetFiles(path);
                }
            }
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
            if (dropInfo.Data is FileModel && dropInfo.TargetItem is FileModel)
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
            FileModel sourceItem = dropInfo.Data as FileModel;
            FileModel targetItem = dropInfo.TargetItem as FileModel;
            
            // targetItem.Add(sourceItem);
            if (this.copyDialog.DataContext is CopyDialogViewModel dialogViewModel)
            {
                dialogViewModel.Source = (dropInfo.Data as FileModel)?.FullName;
                dialogViewModel.Target = (dropInfo.TargetItem as FileModel)?.FullName;
            }

            this.copyDialog.ShowDialog();
        }
    }
}
