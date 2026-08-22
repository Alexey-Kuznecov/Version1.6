namespace UnityCommander.Diagnostics.Performance
{
    public interface IPerformanceComparisonFormatter
    {
        IEnumerable<string> Format(
            string FirstName,
            string SecondName,
            IReadOnlyList<PerformanceComparison> comparisons);
    }
}