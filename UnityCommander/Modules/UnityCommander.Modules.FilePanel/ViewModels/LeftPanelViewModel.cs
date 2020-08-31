using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.IO;
using UnityCommander.Business;
using GongSolutions.Wpf.DragDrop;
using System.Windows;
using UnityCommander.Modules.FilePanel.Views;

namespace UnityCommander.Modules.FilePanel.ViewModels
{
    public class LeftPanelViewModel : BindableBase, IDropTarget
    {
        private ObservableCollection<FileModel> _fileList;
        private CopyDialogView _copyDialog;

        public ObservableCollection<FileModel> FileList
        {
            get { return _fileList; }
            set { SetProperty(ref _fileList, value); }
        }

        public LeftPanelViewModel()
        {
            this._copyDialog = new CopyDialogView();

            FileList = new ObservableCollection<FileModel>();

            DirectoryInfo directoryInfo = new DirectoryInfo("h:\\Works\\UnitTests");

            foreach (var item in directoryInfo.GetDirectories())
            {
                FileList.Add(new FileModel
                {
                    FileName = item.Name,
                    FullName = item.FullName
                });
            }
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
            CopyDialogViewModel dialogViewModel = this._copyDialog.DataContext as CopyDialogViewModel;
            dialogViewModel.Source = (dropInfo.Data as FileModel).FullName;
            dialogViewModel.Target = (dropInfo.TargetItem as FileModel).FullName;
            this._copyDialog.ShowDialog();
        }
    }
}
