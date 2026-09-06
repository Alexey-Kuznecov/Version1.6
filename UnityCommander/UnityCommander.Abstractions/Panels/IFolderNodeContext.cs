
using System.Collections.ObjectModel;

namespace UnityCommander.Abstractions.Panels
{
    public interface IFolderNodeContext
    {
        ObservableCollection<IFolderItem> Folders { get; }

        IFolderItem? Find(string path);

        void Add(IFolderItem file);

        bool Remove(string path);

        bool Update(IFolderItem file);

        bool Rename(string path, string newPath);
    }
}
