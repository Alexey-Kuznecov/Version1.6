
namespace UnityCommander.Diagnostics.Performance
{
    public interface IPerformanceTableFormatter
    {
        IEnumerable<string> Format(
            IReadOnlyList<PerformanceMeasurement> measurements);
        IEnumerable<string> Format(IReadOnlyList<PerformanceItemStatistics> stats);
    }
}
