
using Prism.Mvvm;
using System.Collections.ObjectModel;
using UnityCommander.Common.Models.Directory;
using UnityCommander.Common.Selection;
using UnityCommander.Modules.FilePanel.Layout;
using UnityCommander.Modules.FilePanel.ViewModels;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.Modules.FilePanel.States
{
    public class PanelState : BindableBase
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

        public ContentViewType DirectoryViewType { get; set; }

        public ObservableCollection<MenuItemViewModel> ContextMenuItems { get; }
           = new();
    }
}
