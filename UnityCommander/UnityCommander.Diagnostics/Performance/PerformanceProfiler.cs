
using System.Collections.Concurrent;
using System.Diagnostics;

namespace UnityCommander.Diagnostics.Performance
{
    public sealed class PerformanceProfiler : IPerformanceProfiler
    {
        private readonly ConcurrentQueue<PerformanceMeasurement> _measurements = new();

        private long _sequence;

        public long CurrentSequence 
            => Volatile.Read(ref _sequence);

        public void Clear()
        {
            while (_measurements.TryDequeue(out _))
            {
            }
        }

        internal IReadOnlyList<PerformanceMeasurement> GetMeasurements(
            long fromSequence)
        {
            return _measurements
                .Where(x => x.Sequence > fromSequence)
                .ToArray();
        }

        internal IReadOnlyList<PerformanceMeasurement> GetMeasurements(
            long fromSequence,
            long toSequence)
        {
            return _measurements
                .Where(x =>
                    x.Sequence > fromSequence &&
                    x.Sequence <= toSequence)
                .ToArray();
        }

        internal IReadOnlyList<PerformanceMeasurement> GetMeasurements(
            string operation)
        {
            return _measurements
                .Where(x => string.Equals(
                    x.Operation,
                    operation,
                    StringComparison.Ordinal))
                .ToArray();
        }

        //internal IReadOnlyList<PerformanceMeasurement> GetMeasurements(
        //    string operation,
        //    long fromSequence,
        //    long toSequence)
        //{
        //    return _measurements
        //        .Where(x =>
        //            string.Equals(
        //                x.Operation,
        //                operation,
        //                StringComparison.Ordinal) &&
        //            x.Sequence > fromSequence &&
        //            x.Sequence <= toSequence)
        //        .ToArray();
        //}

        internal IReadOnlyList<string> GetOperations()
        {
            return _measurements
                .Select(x => x.Operation)
                .Distinct(StringComparer.Ordinal)
                .OrderBy(x => x)
                .ToArray();
        }

        public IPerformanceScope Measure(
            string operation)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(operation);

            return new Scope(
                operation,
                _measurements,
                this);
        }

        private sealed class Scope : IPerformanceScope
        {
            private readonly PerformanceProfiler _profiler;

            private readonly string _operation;
            private readonly ConcurrentQueue<PerformanceMeasurement> _measurements;
            private readonly Stopwatch _stopwatch;

            private readonly Dictionary<string, object?> _metadata = new();

            private int _disposed;

            public Scope(
                string operation,
                ConcurrentQueue<PerformanceMeasurement> measurements,
                PerformanceProfiler profiler)
            {
                _operation = operation;
                _measurements = measurements;
                _profiler = profiler;

                _stopwatch = Stopwatch.StartNew();
            }

            public void SetMetadata(
                string key,
                object? value)
            {
                if (_disposed != 0)
                    return;

                _metadata[key] = value;
            }

            public void Dispose()
            {
                if (Interlocked.Exchange(ref _disposed, 1) != 0)
                    return;

                _stopwatch.Stop();

                var sequence =
                    Interlocked.Increment(ref _profiler._sequence);

                var measurement =
                    new PerformanceMeasurement(
                        sequence,
                        _operation,
                        _stopwatch.Elapsed,
                        DateTime.UtcNow,
                        new Dictionary<string, object?>(
                            _metadata));

                _measurements.Enqueue(measurement);
            }
        }
    }
}
