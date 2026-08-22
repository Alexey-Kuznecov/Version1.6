
namespace UnityCommander.Diagnostics.Performance
{
    public sealed record PerformanceSnapshot(
     string Name,
     DateTime CreatedAt,
     long FromSequence,
     long ToSequence,
     IReadOnlyDictionary<string, PerformanceStatistics> Statistics);
}
