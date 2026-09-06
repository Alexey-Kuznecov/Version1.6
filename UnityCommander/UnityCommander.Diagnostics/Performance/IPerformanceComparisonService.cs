namespace UnityCommander.Diagnostics.Performance
{
    public interface IPerformanceComparisonService
    {
        public IReadOnlyList<PerformanceComparison> Compare(
            string firstSnapshot,
            string secondSnapshot);
    }
}