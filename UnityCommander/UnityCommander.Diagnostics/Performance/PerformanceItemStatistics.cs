
namespace UnityCommander.Diagnostics.Performance
{
    public sealed record PerformanceItemStatistics(
      PerformanceItemRange Range,
          int Samples,
          TimeSpan Total,
          TimeSpan Average,
          TimeSpan P95,
          TimeSpan Min,
          TimeSpan Max);
}
