
using UnityCommander.Diagnostics.Abstraction.Performance;

namespace UnityCommander.Diagnostics.Workbench.Models;

public sealed class SnapshotItem
{
    public SnapshotItem(
        Guid recordId,
        PerformanceSnapshot data)
    {
        RecordId = recordId;
        Data = data;
    }

    public Guid RecordId { get; }

    public PerformanceSnapshot Data { get; }

    public string Name => Data.Name;

    public DateTime CreatedAt =>
        Data.CreatedAt.Kind == DateTimeKind.Utc
            ? Data.CreatedAt.ToLocalTime()
            : Data.CreatedAt;

    public long FromSequence => Data.FromSequence;

    public long ToSequence => Data.ToSequence;

    public int OperationCount => Data.Statistics.Count;

    public int MeasurementCount =>
        Data.Statistics.Values.Sum(x => x.Count);

    public TimeSpan TotalMeasureTime =>
    Data.Statistics.Values.Aggregate(
        TimeSpan.Zero,
        (total, statistics) => total + statistics.Total);

    public string TotalMeasureTimeDisplay =>
        $"{TotalMeasureTime.TotalMilliseconds:N2} ms";

    public string Description =>
        $"{MeasurementCount:N0} measurements across " +
        $"{OperationCount:N0} operations";
}
