
using System.Collections.ObjectModel;
using UnityCommander.Common.Models.Directory;

namespace UnityCommander.Modules.FilePanel.States
{
    public class FileNodeContext : BaseNodeContext
    {
        private ObservableCollection<FileModel> _files = new();

        public ObservableCollection<FileModel> Files
        {
            get => _files;
            set => SetProperty(ref _files, value);
        }

        public FileModel SelectedFile { get; set; }

        public string CurrentPath => SelectedFile.Path;
    }
}
