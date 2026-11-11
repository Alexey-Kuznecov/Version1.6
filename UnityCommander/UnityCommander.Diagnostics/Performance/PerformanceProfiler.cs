using System.Collections.Concurrent;
using System.Diagnostics;
using UnityCommander.Diagnostics.Abstraction;
using UnityCommander.Diagnostics.Abstraction.Performance;
using UnityCommander.Diagnostics.Performance;

public sealed class PerformanceProfiler : IPerformanceProfiler
{
    private readonly ConcurrentQueue<PerformanceMeasurement> _measurements = new();

    private readonly IDiagnosticsRecorder _recorder;

    private long _sequence;

    public long CurrentSequence
        => Volatile.Read(ref _sequence);

    public PerformanceProfiler(IDiagnosticsRecorder recorder)
    {
        _recorder = recorder;
    }

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

    public IReadOnlyList<PerformanceMeasurement> GetMeasurements(
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

    internal IReadOnlyList<string> GetOperations()
    {
        return _measurements
            .Select(x => x.Operation)
            .Distinct(StringComparer.Ordinal)
            .OrderBy(x => x)
            .ToArray();
    }

    public IPerformanceScope Measure(string operation)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(operation);

        return new Scope(operation, this);
    }

    private void RecordMeasurement(
        PerformanceMeasurement measurement)
    {
        // Сохраняем измерение во внутренней очереди профайлера.
        _measurements.Enqueue(measurement);

        // Передаём измерение центральной системе диагностики.
        var record = new DiagnosticRecord(
            DiagnosticRecordKind.Measurement,
            new DateTimeOffset(measurement.Timestamp),
            "Performance",
            measurement.Operation,
            measurement,
            Duration: measurement.Duration);

        _recorder.TryRecord(record);
    }

    private sealed class Scope : IPerformanceScope
    {
        private readonly PerformanceProfiler _profiler;
        private readonly string _operation;
        private readonly Stopwatch _stopwatch;

        private readonly Dictionary<string, object?> _metadata = new();

        private int _disposed;

        public Scope(
            string operation,
            PerformanceProfiler profiler)
        {
            _operation = operation;
            _profiler = profiler;

            _stopwatch = Stopwatch.StartNew();
        }

        public void SetMetadata(
            string key,
            object? value)
        {
            if (_disposed != 0)
            {
                return;
            }

            _metadata[key] = value;
        }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref _disposed, 1) != 0)
            {
                return;
            }

            _stopwatch.Stop();

            var sequence =
                Interlocked.Increment(ref _profiler._sequence);

            var measurement =
                new PerformanceMeasurement(
                    sequence,
                    _operation,
                    _stopwatch.Elapsed,
                    DateTime.UtcNow,
                    new Dictionary<string, object?>(_metadata));

            _profiler.RecordMeasurement(measurement);
        }
    }
}