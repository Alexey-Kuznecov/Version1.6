
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityCommander.Abstractions.Panels;

namespace UnityCommander.Modules.FilePanel.States
{
    public sealed class UnifiedNodeContext
    {
        public FolderNodeContext Folder { get; }
        public FileNodeContext File { get; }

        public ObservableCollection<IDirectoryItem> Items { get; } = [];

        public UnifiedNodeContext(
            FolderNodeContext folder,
            FileNodeContext file)
        {
            Folder = folder;
            File = file;

            RebuildItems();
        }

        public void RebuildItems()
        {
            Items.Clear();

            foreach (var item in Folder.Folders)
                Items.Add(item);

            foreach (var item in File.Files)
                Items.Add(item);
        }
    }
}
