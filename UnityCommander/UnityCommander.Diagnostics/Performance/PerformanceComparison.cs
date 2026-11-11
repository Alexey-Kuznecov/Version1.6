
using UnityCommander.Diagnostics.Abstraction.Performance;

namespace UnityCommander.Diagnostics.Performance
{
    public sealed record PerformanceComparison(
     string Operation,
     PerformanceStatistics First,
     PerformanceStatistics Second);
}
