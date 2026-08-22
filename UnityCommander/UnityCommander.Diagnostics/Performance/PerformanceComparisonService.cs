
namespace UnityCommander.Diagnostics.Performance
{
    public sealed class PerformanceComparisonService
     : IPerformanceComparisonService
    {
        private readonly IPerformanceSnapshotService _snapshots;
        private readonly IPerformanceAnalyzer _analyzer;
        private readonly PerformanceProfiler _profiler;

        public PerformanceComparisonService(
            IPerformanceSnapshotService snapshots,
            IPerformanceAnalyzer analyzer,
            IPerformanceProfiler profiler)
        {
            _snapshots = snapshots;
            _analyzer = analyzer;
            _profiler = (PerformanceProfiler)profiler;
        }

        public IReadOnlyList<PerformanceComparison> Compare(
          string firstSnapshot,
          string secondSnapshot)
        {
            var first =
                _snapshots.Get(firstSnapshot)
                ?? throw new InvalidOperationException(
                    $"Snapshot '{firstSnapshot}' not found.");


            var second =
                _snapshots.Get(secondSnapshot)
                ?? throw new InvalidOperationException(
                    $"Snapshot '{secondSnapshot}' not found.");


            var operations =
                first.Statistics.Keys
                    .Concat(second.Statistics.Keys)
                    .Distinct(StringComparer.Ordinal)
                    .ToArray();


            var result =
                new List<PerformanceComparison>();


            foreach (var operation in operations)
            {
                if (!first.Statistics.TryGetValue(
                        operation,
                        out var firstStats))
                {
                    continue;
                }


                if (!second.Statistics.TryGetValue(
                        operation,
                        out var secondStats))
                {
                    continue;
                }


                result.Add(
                    new PerformanceComparison(
                        operation,
                        firstStats,
                        secondStats));
            }


            return result;
        }
    }
}