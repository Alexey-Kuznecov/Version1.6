
namespace UnityCommander.Diagnostics.Abstraction.Performance
{
    public sealed record PerformanceMeasurement(
     long Sequence,
     string Operation,
     TimeSpan Duration,
     DateTime Timestamp,
     IReadOnlyDictionary<string, object?> Metadata);
}
