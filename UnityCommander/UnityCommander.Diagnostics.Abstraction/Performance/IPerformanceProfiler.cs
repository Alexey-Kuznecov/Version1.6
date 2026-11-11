
namespace UnityCommander.Diagnostics.Abstraction.Performance
{
    public interface IPerformanceProfiler
    {
        IPerformanceScope Measure(
            string operation);

        long CurrentSequence { get; }

        IReadOnlyList<PerformanceMeasurement> GetMeasurements(
            long fromSequence,
            long toSequence);

        void Clear();
    }
}
