
namespace UnityCommander.Diagnostics.Performance
{
    public sealed record PerformanceStatistics(
     string Operation,
     int Count,
     TimeSpan Total,
     TimeSpan Average,
     TimeSpan P95,
     TimeSpan Min,
     TimeSpan Max);
}
