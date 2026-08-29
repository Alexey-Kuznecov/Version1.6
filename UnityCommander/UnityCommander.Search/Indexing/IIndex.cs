
using UnityCommander.Search.Models;

namespace UnityCommander.Search.Indexing
{
    public interface IIndex
    {
        bool TryGet(string path, out SearchItem item);
    }
}
