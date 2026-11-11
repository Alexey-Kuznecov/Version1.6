
namespace UnityCommander.Diagnostics.Abstraction.Performance
{
    public sealed record PerformanceSnapshot(
     string Name,
     DateTime CreatedAt,
     long FromSequence,
     long ToSequence,
     IReadOnlyDictionary<string, PerformanceStatistics> Statistics);
}
