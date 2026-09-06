
using UnityCommander.Index.Models;

namespace UnityCommander.Index.Abstractions
{
    public interface IFileIndexChangeQueue
    {
        void Enqueue(IndexChange change);

        bool TryDequeue(out IndexChange? change);

        int Count { get; }
    }
}
