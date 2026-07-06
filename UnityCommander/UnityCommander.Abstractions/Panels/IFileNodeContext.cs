
using System.Collections.ObjectModel;

namespace UnityCommander.Abstractions.Panels
{
    public interface IFileNodeContext
    {
        ObservableCollection<IFileItem> Files { get; }

        IFileItem? Find(string path);

        void Add(IFileItem file);

        bool Remove(string path);

        bool Update(IFileItem file);
    }
}
