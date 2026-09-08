
using System;
using System.Collections.ObjectModel;
using System.IO;
using UnityCommander.Modules.LeftSideBars.Widget.Models;

namespace UnityCommander.Modules.LeftSideBars.Widget
{
    public class SystemFoldersViewModel
    {
        public ObservableCollection<SystemFolderInfoModel> Folders { get; } = [];

        public SystemFoldersViewModel()
        {
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
                IconKey = "image"
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
