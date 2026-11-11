namespace UnityCommander.Diagnostics.Workbench.Models;

public sealed class TraceEventItem
{
    public TraceEventItem(
        DateTime timestamp,
        string eventName,
        string source,
        string traceId,
        int depth,
        double? durationMilliseconds,
        string data,
        string? spanId = null,
        string? parentSpanId = null,
        string? pluginId = null)
    {
        Timestamp = timestamp;
        EventName = eventName;
        Source = source;
        TraceId = traceId;
        Depth = depth;
        DurationMilliseconds = durationMilliseconds;
        Data = data;
        SpanId = spanId;
        ParentSpanId = parentSpanId;
        PluginId = pluginId;
    }

    public DateTime Timestamp { get; }

    public string EventName { get; }

    public string Source { get; }

    public string TraceId { get; }

    public string? SpanId { get; }

    public string? ParentSpanId { get; }

    public int Depth { get; }

    public double? DurationMilliseconds { get; }

    public string Data { get; }

    public string? PluginId { get; }
}