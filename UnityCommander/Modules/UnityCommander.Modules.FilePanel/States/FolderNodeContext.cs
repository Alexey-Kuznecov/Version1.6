
using System.Collections.ObjectModel;
using System.Windows.Input;
using UnityCommander.Common.Models.Directory;

namespace UnityCommander.Modules.FilePanel.States
{
    public class FolderNodeContext : BaseNodeContext
    {
        private ObservableCollection<FolderModel> _folders = new ();

        public ObservableCollection<FolderModel> Folders
        {
            get => _folders;
            set => SetProperty(ref _folders, value);
        }

        public FolderModel SelectedFolder { get; set; }

        public ICommand NavigateCommand { get; set; }
    }
}
