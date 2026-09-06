
namespace UnityCommander.Index.Models
{
    public sealed class IndexOperationResult
    {
        public int Added { get; set; }
        public long? RootId { get; set; }
        public List<IndexedFile>? Items { get; set; }
    }
}
