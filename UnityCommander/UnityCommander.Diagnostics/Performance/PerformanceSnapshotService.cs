
using UnityCommander.Diagnostics.Abstraction;
using UnityCommander.Diagnostics.Abstraction.Performance;

namespace UnityCommander.Diagnostics.Performance
{
    public sealed class PerformanceSnapshotService
      : IPerformanceSnapshotService
    {
        private readonly IPerformanceProfiler _profiler;
        private readonly IPerformanceAnalyzer _analyzer;
        private readonly IDiagnosticsRecorder _recorder;

        private readonly List<PerformanceSnapshot> _snapshots = new();

        private readonly object _sync = new();

        private long _lastSequence;

        public PerformanceSnapshotService(
            IPerformanceProfiler profiler,
            IPerformanceAnalyzer analyzer,
            IDiagnosticsRecorder recorder)
        {
            _profiler = profiler;
            _analyzer = analyzer;
            _recorder = recorder;
        }

        public PerformanceSnapshot Create(string name)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);

            name = name.Trim();

            lock (_sync)
            {
                EnsureActiveSession();

                long toSequence = _profiler.CurrentSequence;

                // Профайлер мог быть очищен отдельно от SnapshotService.
                // В таком случае начинаем отсчёт заново.
                if (toSequence < _lastSequence)
                {
                    _lastSequence = 0;
                }

                long fromSequence = _lastSequence;

                var measurements = _profiler
                    .GetMeasurements(fromSequence, toSequence)
                    .ToArray();

                var statistics = BuildStatistics(measurements);

                DateTime createdAt = DateTime.UtcNow;

                var snapshot = new PerformanceSnapshot(
                    name,
                    createdAt,
                    fromSequence,
                    toSequence,
                    statistics);

                SaveSnapshot(snapshot);

                // Обновляем локальную историю и контрольную точку
                // только после принятия записи Recorder.
                _snapshots.Add(snapshot);

                _lastSequence = toSequence;

                return snapshot;
            }
        }

        public IReadOnlyList<PerformanceSnapshot> GetAll()
        {
            lock (_sync)
            {
                return _snapshots.ToArray();
            }
        }

        public PerformanceSnapshot? Get(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }

            lock (_sync)
            {
                return _snapshots.LastOrDefault(
                    x => string.Equals(
                        x.Name,
                        name,
                        StringComparison.OrdinalIgnoreCase));
            }
        }

        public void Clear()
        {
            lock (_sync)
            {
                _snapshots.Clear();

                _lastSequence = _profiler.CurrentSequence;
            }
        }

        private void EnsureActiveSession()
        {
            if (_recorder.CurrentSession == null)
            {
                throw new InvalidOperationException(
                    "Cannot create a performance snapshot without an active diagnostic session.");
            }
        }

        private Dictionary<string, PerformanceStatistics> BuildStatistics(
            IReadOnlyList<PerformanceMeasurement> measurements)
        {
            var statistics = new Dictionary<string, PerformanceStatistics>(
                StringComparer.Ordinal);

            var operations = measurements
                .Select(x => x.Operation)
                .Distinct(StringComparer.Ordinal);

            foreach (var operation in operations)
            {
                var result = _analyzer.GetStatistics(
                    operation,
                    measurements);

                if (result != null)
                {
                    statistics[operation] = result;
                }
            }

            return statistics;
        }

        private void SaveSnapshot(PerformanceSnapshot snapshot)
        {
            var record = new DiagnosticRecord(
                DiagnosticRecordKind.Snapshot,
                new DateTimeOffset(snapshot.CreatedAt, TimeSpan.Zero),
                "Performance",
                snapshot.Name,
                snapshot);

            if (!_recorder.TryRecord(record))
            {
                throw new InvalidOperationException(
                    $"The diagnostics recorder rejected snapshot '{snapshot.Name}'.");
            }
        }
    }
}
