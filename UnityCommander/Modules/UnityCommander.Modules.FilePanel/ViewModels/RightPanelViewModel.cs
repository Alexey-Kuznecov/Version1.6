using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.IO;
using UnityCommander.Business;
using GongSolutions.Wpf.DragDrop;
using System.Windows;

namespace UnityCommander.Modules.FilePanel.ViewModels
{
    public class RightPanelViewModel : BindableBase, IDropTarget
    {
        public RightPanelViewModel()
        {
            FileList = new ObservableCollection<FileModel>();

            DirectoryInfo directoryInfo = new DirectoryInfo("g:\\Program Files\\Total Commander 8.52a\\Doc\\");

            foreach (var item in directoryInfo.GetDirectories())
            {
                FileList.Add(new FileModel 
                { 
                    FileName = item.Name, 
                    FullName = item.FullName 
                });
            }
        }

        private ObservableCollection<FileModel> _fileList;

        public ObservableCollection<FileModel> FileList
        {
            get { return _fileList; }
            set { SetProperty(ref _fileList, value); }
        }

        void IDropTarget.DragOver(IDropInfo dropInfo)
        {
            FileModel sourceItem = dropInfo.Data as FileModel;
            FileModel targetItem = dropInfo.TargetItem as FileModel;

            if (sourceItem != null && targetItem != null)
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                dropInfo.Effects = DragDropEffects.Copy;
            }
        }

        void IDropTarget.Drop(IDropInfo dropInfo)
        {
            FileModel sourceItem = dropInfo.Data as FileModel;
            FileModel targetItem = dropInfo.TargetItem as FileModel;
            //targetItem.Add(sourceItem);
        }
    }
}
