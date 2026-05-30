
using System;
using System.Collections.ObjectModel;
using UnityCommander.Common.Models.Directory;
using UnityCommander.Modules.FilePanel.ViewModels;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.Modules.FilePanel.States
{
    public class TabState :
        IColumnSource<FileModel>,
        IColumnSource<FolderModel>
    {
        public string CurrentDirectory { get; set; }

        public ObservableCollection<FolderModel> Directories { get; }
            = new();

        public ObservableCollection<FileModel> Files { get; }
            = new();

        public ObservableCollection<DriveModel> Drives { get; }
            = new();

        public object SelectedDirectory { get; set; }

        public FileModel CurrentFile { get; set; }

        public ObservableCollection<BaseDirectory> SelectedItems { get; set; }
             = new();

        public PanelMode Mode { get; set; }

        public ObservableCollection<MenuItemViewModel> ContextMenuItems { get; }
           = new();

        public BaseDirectory SelectedCurrentDirectoryItem { get; set; }

        public ISelectionManager SelectionManager { get; set; }

        public Guid Token { get; set; }

        ObservableCollection<FileModel> IColumnSource<FileModel>.Items => Files;
        ObservableCollection<FolderModel> IColumnSource<FolderModel>.Items => Directories;
    }
}
