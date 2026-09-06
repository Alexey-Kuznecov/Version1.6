
namespace UnityCommander.Search.Models
{
    public sealed class SearchProgress
    {
        public long Processed { get; init; }
        public long Found { get; init; }
        public long Skipped { get; init; }
        public long? Total { get; init; }
    }
}
