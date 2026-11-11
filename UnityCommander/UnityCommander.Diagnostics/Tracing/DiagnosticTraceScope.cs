
using System.Diagnostics;
using UnityCommander.Diagnostics.Tracing;

public sealed class DiagnosticTraceScope : IDiagnosticTraceScope
{
    private readonly object _sync = new();

    private readonly DiagnosticTrace _trace;
    private readonly string _source;
    private readonly DiagnosticTraceContext _context;
    private readonly Stopwatch _stopwatch;

    private bool _completed;

    public string Id => _context.SpanId;

    public DiagnosticTraceScope(
        DiagnosticTrace trace,
        string source,
        string operation,
        DiagnosticTraceContext context,
        IReadOnlyDictionary<string, object?>? data)
    {
        _trace = trace;
        _source = source;
        _context = context;

        _stopwatch = Stopwatch.StartNew();

        _trace.Write(
            _context,
            _source,
            $"{operation}.begin",
            data);
    }

    public void Write(
        string eventName,
        IReadOnlyDictionary<string, object?>? data = null)
    {
        lock (_sync)
        {
            if (_completed)
            {
                return;
            }

            _trace.Write(
                _context,
                _source,
                eventName,
                data);
        }
    }

    public void Complete(
        IReadOnlyDictionary<string, object?>? data = null)
    {
        Finish(
            "complete",
            data);
    }

    public void Fail(
        Exception exception,
        IReadOnlyDictionary<string, object?>? data = null)
    {
        ArgumentNullException.ThrowIfNull(exception);

        var result = new Dictionary<string, object?>
        {
            ["exception"] = exception.GetType().FullName,
            ["message"] = exception.Message
        };

        if (data != null)
        {
            foreach (var pair in data)
            {
                result[pair.Key] = pair.Value;
            }
        }

        Finish(
            "failed",
            result);
    }

    public void Dispose()
    {
        Complete();
    }

    private void Finish(
        string eventName,
        IReadOnlyDictionary<string, object?>? data)
    {
        lock (_sync)
        {
            if (_completed)
            {
                return;
            }

            _completed = true;
            _stopwatch.Stop();

            try
            {
                _trace.Write(
                    _context,
                    _source,
                    eventName,
                    data,
                    _stopwatch.Elapsed.Ticks);
            }
            finally
            {
                _trace.OnScopeCompleted(_context);
            }
        }
    }
}