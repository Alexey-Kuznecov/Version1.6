
namespace UnityCommander.Diagnostics.Performance
{
    public interface IPerformanceAnalyzer
    {
        PerformanceStatistics? GetStatistics(
           string operation,
           IReadOnlyList<PerformanceMeasurement> measurements);

        PerformanceStatistics? GetStatistics(
            string operation);

        IReadOnlyList<PerformanceMeasurement> GetSlowest(
            string operation,
            int count);
        
        IReadOnlyList<string> GetOperations();

        public IReadOnlyList<PerformanceMeasurement> GetRecent(
            string operation,
            int count);

        IReadOnlyList<PerformanceItemStatistics> GetStatisticsByItems(
            string operation);
    }
}
