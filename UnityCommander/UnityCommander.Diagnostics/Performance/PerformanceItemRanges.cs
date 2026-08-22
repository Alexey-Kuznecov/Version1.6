
namespace UnityCommander.Diagnostics.Performance
{
    public static class PerformanceItemRanges
    {
        public static IReadOnlyList<PerformanceItemRange> Default { get; } =
        [
            new(0, 10),
            new(11, 100),
            new(101, 500),
            new(501, 1_000),
            new(1_001, 5_000),
            new(5_001, 10_000),
            new(10_001, null)
        ];
    }
}
