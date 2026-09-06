
namespace UnityCommander.Diagnostics.Performance
{
    public sealed class PerformanceSnapshotService
     : IPerformanceSnapshotService
    {
        private readonly PerformanceProfiler _profiler;
        private readonly IPerformanceAnalyzer _analyzer;

        private readonly List<PerformanceSnapshot> _snapshots = [];

        private long _lastSequence;

        public PerformanceSnapshotService(
            IPerformanceProfiler profiler,
            IPerformanceAnalyzer analyzer)
        {
            _profiler = (PerformanceProfiler)profiler;
            _analyzer = analyzer;
        }

        public PerformanceSnapshot Create(
            string name)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);

            var fromSequence = _lastSequence;
            var toSequence = _profiler.CurrentSequence;

            var measurements =
                _profiler.GetMeasurements(
                    fromSequence,
                    toSequence);

            var operations =
                measurements
                    .Select(x => x.Operation)
                    .Distinct(StringComparer.Ordinal)
                    .ToArray();

            var statistics =
                new Dictionary<string, PerformanceStatistics>(
                    StringComparer.Ordinal);

            foreach (var operation in operations)
            {
                var stats =
                    _analyzer.GetStatistics(
                        operation,
                        measurements);

                if (stats is not null)
                {
                    statistics[operation] = stats;
                }
            }

            var snapshot =
                new PerformanceSnapshot(
                    name,
                    DateTime.UtcNow,
                    fromSequence,
                    toSequence,
                    statistics);

            _snapshots.Add(snapshot);

            _lastSequence = toSequence;

            return snapshot;
        }

        public IReadOnlyList<PerformanceSnapshot> GetAll() =>
            _snapshots;

        public PerformanceSnapshot? Get(string name) =>
            _snapshots.FirstOrDefault(
                x => string.Equals(
                    x.Name,
                    name,
                    StringComparison.OrdinalIgnoreCase));

        public void Clear()
        {
            _snapshots.Clear();
            _lastSequence = _profiler.CurrentSequence;
        }
    }
}
