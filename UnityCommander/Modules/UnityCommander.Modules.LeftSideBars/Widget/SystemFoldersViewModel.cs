
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.IO;
using UnityCommander.Modules.LeftSideBars.Widget.Models;
using UnityCommander.Services;

namespace UnityCommander.Modules.LeftSideBars.Widget
{
    public class SystemFoldersViewModel : BindableBase
    {
        private ActiveNavigationService _executor;

        private SystemFolderInfoModel _selectedFolder;

        public ObservableCollection<SystemFolderInfoModel> Folders { get; } = [];

        public SystemFolderInfoModel SelectedSystemFolder
        {
            get => _selectedFolder;
            set
            {
                if (SetProperty(ref _selectedFolder, value))
                {
                    _executor.Navigate(_selectedFolder.Path);
                }
            }
        }

        public SystemFoldersViewModel(ActiveNavigationService executor)
        {
            _executor = executor;
            Folders.Add(new SystemFolderInfoModel
            {
                Name = "Desktop",
                Path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                IconKey = "desktop"
            });

            Folders.Add(new SystemFolderInfoModel
            {
                Name = "Documents",
                Path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                IconKey = "document"
            });

            Folders.Add(new SystemFolderInfoModel
            {
                Name = "Downloads",
                Path = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    "Downloads"),
                IconKey = "download"
            });

            Folders.Add(new SystemFolderInfoModel
            {
                Name = "Pictures",
                Path = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
                IconKey = "picture"
            });

            Folders.Add(new SystemFolderInfoModel
            {
                Name = "Music",
                Path = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic),
                IconKey = "music"
            });

            Folders.Add(new SystemFolderInfoModel
            {
                Name = "Videos",
                Path = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos),
                IconKey = "video"
            });
        }
    }
}
