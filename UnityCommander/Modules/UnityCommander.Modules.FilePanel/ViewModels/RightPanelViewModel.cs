// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RightPanelViewModel.cs" company="T">
// Copyright (p) Alexey Kuznecov. All right reserved.  
// </copyright>
// <summary>
//   The right panel view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace UnityCommander.Modules.FilePanel.ViewModels
{
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Windows;
    using GongSolutions.Wpf.DragDrop;
    using Prism.Mvvm;
    using UnityCommander.Business;

    /// <summary>
    /// The right panel view model.
    /// </summary>
    public class RightPanelViewModel : BindableBase, IDropTarget
    {
        /// <summary>
        /// The _file list.
        /// </summary>
        private ObservableCollection<FileModel> fileList;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="RightPanelViewModel"/> class.
        /// </summary>
        public RightPanelViewModel()
        {
            this.FileList = new ObservableCollection<FileModel>();

            DirectoryInfo directoryInfo = new DirectoryInfo("h:\\");

            foreach (var item in directoryInfo.GetDirectories())
            {
                this.FileList.Add(new FileModel 
                { 
                    FileName = item.Name, 
                    FullName = item.FullName 
                });
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
        }
    }
}
