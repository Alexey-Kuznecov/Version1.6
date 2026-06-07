
namespace UnityCommander.Core.Performance
{
    public sealed class CacheStatistics
    {
        public int Count { get; init; }

        public long Hits { get; init; }

        public long Misses { get; init; }

        public double HitRate { get; init; }
    }
}
