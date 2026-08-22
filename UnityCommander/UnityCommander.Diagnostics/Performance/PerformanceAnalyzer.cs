
namespace UnityCommander.Diagnostics.Performance
{
    public sealed class PerformanceAnalyzer
      : IPerformanceAnalyzer
    {
        private readonly PerformanceProfiler _profiler;

        public PerformanceAnalyzer(
            IPerformanceProfiler profiler)
        {
            _profiler = (PerformanceProfiler)profiler;
        }

        public PerformanceStatistics? GetStatistics(
            string operation,
            IReadOnlyList<PerformanceMeasurement> measurements)
        {
            var filtered = measurements
                .Where(x => string.Equals(
                    x.Operation,
                    operation,
                    StringComparison.Ordinal))
                .ToArray();

            return filtered.Length == 0
                ? null
                : CalculateStatistics(operation, filtered);
        }

        public PerformanceStatistics? GetStatistics(
            string operation)
        {
            return CalculateStatistics(
                operation,
                _profiler.GetMeasurements(operation));
        }

        public IReadOnlyList<PerformanceMeasurement> GetSlowest(
            string operation,
            int count)
        {
            return _profiler
                .GetMeasurements(operation)
                .OrderByDescending(x => x.Duration)
                .Take(count)
                .ToArray();
        }

        public IReadOnlyList<string> GetOperations()
        {
            return _profiler.GetOperations();
        }

        public IReadOnlyList<PerformanceMeasurement> GetRecent(
            string operation,
            int count)
        {
            return _profiler
                .GetMeasurements(operation)
                .Where(x => string.Equals(
                    x.Operation,
                    operation,
                    StringComparison.Ordinal))
                .OrderByDescending(x => x.Timestamp)
                .Take(count)
                .ToArray();
        }

        private static PerformanceStatistics? CalculateStatistics(
            string operation,
            IReadOnlyList<PerformanceMeasurement> measurements)
        {
            var filtered = measurements
                .Where(x => string.Equals(
                    x.Operation,
                    operation,
                    StringComparison.Ordinal))
                .ToArray();

            if (filtered.Length == 0)
                return null;

            var durations = filtered
                .Select(x => x.Duration)
                .OrderBy(x => x)
                .ToArray();

            var total = TimeSpan.FromTicks(
                durations.Sum(x => x.Ticks));

            return new PerformanceStatistics(
                operation,
                filtered.Length,
                total,
                TimeSpan.FromTicks(
                    total.Ticks / filtered.Length),
                CalculatePercentile(durations, 0.95),
                durations[0],
                durations[^1]);
        }

        private static TimeSpan CalculatePercentile(
             IReadOnlyList<TimeSpan> sorted,
             double percentile)
        {
            if (sorted.Count == 0)
                return TimeSpan.Zero;

            var position =
                (sorted.Count - 1) * percentile;

            var lower = (int)Math.Floor(position);
            var upper = (int)Math.Ceiling(position);

            if (lower == upper)
                return sorted[lower];

            var fraction = position - lower;

            var ticks =
                sorted[lower].Ticks +
                (long)(
                    (sorted[upper].Ticks - sorted[lower].Ticks)
                    * fraction);

            return TimeSpan.FromTicks(ticks);
        }

        public IReadOnlyList<PerformanceItemStatistics>GetStatisticsByItems(string operation)
        {
            var measurements = _profiler
                .GetMeasurements(operation)
                .Where(x => string.Equals(
                    x.Operation,
                    operation,
                    StringComparison.Ordinal))
                .Where(HasItems)
                .ToArray();

            var result = new List<PerformanceItemStatistics>();

            foreach (var range in PerformanceItemRanges.Default)
            {
                var group = measurements
                    .Where(x => range.Contains(GetItems(x)))
                    .ToArray();

                if (group.Length == 0)
                    continue;

                var durations = group
                    .Select(x => x.Duration)
                    .ToArray();

                var total = TimeSpan.FromTicks(
                    durations.Sum(x => x.Ticks));

                result.Add(
                    new PerformanceItemStatistics(
                        range,
                        durations.Length,
                        total,
                        TimeSpan.FromTicks(
                            total.Ticks / durations.Length),
                        CalculatePercentile(
                            durations,
                            0.95),
                        durations.Min(),
                        durations.Max()));
            }

            return result;
        }

        private static bool HasItems(
            PerformanceMeasurement measurement)
        {
            return measurement.Metadata.TryGetValue(
                "Items",
                out var value) &&
                TryGetItems(value, out _);
        }

        private static long GetItems(
            PerformanceMeasurement measurement)
        {
            return TryGetItems(
                measurement.Metadata["Items"],
                out var items)
                ? items
                : 0;
        }

        private static bool TryGetItems(
            object? value,
            out long items)
        {
            switch (value)
            {
                case byte v:
                    items = v;
                    return true;

                case short v:
                    items = v;
                    return true;

                case int v:
                    items = v;
                    return true;

                case long v:
                    items = v;
                    return true;

                default:
                    items = 0;
                    return false;
            }
        }
    }
}
