namespace UnityCommander.Diagnostics.Workbench.Models;

public sealed class PerformanceOperationItem
{
    public PerformanceOperationItem(
        string operation,
        string category,
        string owner,
        int count,
        double totalMilliseconds,
        double averageMilliseconds,
        double p95Milliseconds,
        double minMilliseconds,
        double maxMilliseconds)
    {
        Operation = operation;
        Category = category;
        Owner = owner;
        Count = count;
        TotalMilliseconds = totalMilliseconds;
        AverageMilliseconds = averageMilliseconds;
        P95Milliseconds = p95Milliseconds;
        MinMilliseconds = minMilliseconds;
        MaxMilliseconds = maxMilliseconds;
    }

    public string Operation { get; }

    public string Category { get; }

    public string Owner { get; }

    public int Count { get; }

    public double TotalMilliseconds { get; }

    public double AverageMilliseconds { get; }

    public double P95Milliseconds { get; }

    public double MinMilliseconds { get; }

    public double MaxMilliseconds { get; }
}
