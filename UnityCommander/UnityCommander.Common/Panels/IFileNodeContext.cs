
using System.Collections.ObjectModel;
using UnityCommander.Common.Models.Directory;

namespace UnityCommander.Common.Panels
{
    public interface IFileNodeContext
    {
        ObservableCollection<FileModel> Files { get; }

        FileModel? Find(string path);

        void Add(FileModel file);

        bool Remove(string path);

        bool Update(FileModel file);
    }
}
