
namespace UnityCommander.Diagnostics.Tracing
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using UnityCommander.Diagnostics.Abstraction;

    public sealed class DiagnosticTrace : IDiagnosticTrace
    {
        private readonly IDiagnosticTraceStore _store;

        private readonly AsyncLocal<DiagnosticTraceContext?> _currentContext = new();

        private static readonly IReadOnlyDictionary<string, object?> EmptyData =
            new Dictionary<string, object?>();
        private readonly IDiagnosticsRecorder _recorder;

        public DiagnosticTrace(
            IDiagnosticTraceStore store, 
            IDiagnosticsRecorder recorder)
        {
            _store = store;
            _recorder = recorder;
        }

        public void Write(
            string source,
            string eventName,
            IReadOnlyDictionary<string, object?>? data = null)
        {
            var context = GetCurrentContext();

            if (context is not null)
            {
                Write(
                    context,
                    source,
                    eventName,
                    data);

                return;
            }

            var entry = new DiagnosticTraceEvent
            {
                Timestamp = DateTime.UtcNow,
                TraceId = Guid.NewGuid().ToString("N"),
                SpanId = null,
                ParentSpanId = null,
                Source = source,
                EventName = eventName,
                Depth = 0,
                Data = data ?? EmptyData
            };

            Record(entry);
        }

        public IDiagnosticTraceScope Begin(
            string source,
            string operation,
            IReadOnlyDictionary<string, object?>? data = null)
        {
            var parent = GetCurrentContext();

            var context = new DiagnosticTraceContext(
                traceId: parent?.TraceId ?? Guid.NewGuid().ToString("N"),
                spanId: Guid.NewGuid().ToString("N"),
                parentSpanId: parent?.SpanId,
                depth: parent == null ? 0 : parent.Depth + 1,
                parentContext: parent);

            var scope = new DiagnosticTraceScope(
                this,
                source,
                operation,
                context,
                data);

            _currentContext.Value = context;

            return scope;
        }

        internal void Write(
            DiagnosticTraceContext context,
            string source,
            string eventName,
            IReadOnlyDictionary<string, object?>? data = null,
            long? durationTicks = null)
        {
            var entry = new DiagnosticTraceEvent
            {
                Timestamp = DateTime.UtcNow,
                TraceId = context.TraceId,
                SpanId = context.SpanId,
                ParentSpanId = context.ParentSpanId,
                Source = source,
                EventName = eventName,
                Depth = context.Depth,
                DurationTicks = durationTicks,
                Data = data ?? EmptyData
            };

            _store.Add(entry);

            Record(entry);
        }

        private void Record(DiagnosticTraceEvent entry)
        {
            // Оставляем быстрый доступ к событиям в оперативной памяти.
            _store.Add(entry);

            // Передаём событие в общую систему диагностики.
            // Само сохранение в SQLite выполнит DiagnosticsRecorder.
            var record = new DiagnosticRecord(
                Kind: DiagnosticRecordKind.Trace,
                Timestamp: new DateTimeOffset(entry.Timestamp, TimeSpan.Zero),
                Source: entry.Source,
                Name: entry.EventName,
                Payload: new DiagnosticTracePayload(
                    entry.TraceId,
                    entry.SpanId,
                    entry.ParentSpanId,
                    entry.Depth,
                    entry.Data),
                Duration: entry.DurationTicks.HasValue
                    ? TimeSpan.FromTicks(entry.DurationTicks.Value)
                    : null);

            _recorder.TryRecord(record);
        }

        internal void OnScopeCompleted(
            DiagnosticTraceContext context)
        {
            context.Deactivate();

            if (ReferenceEquals(_currentContext.Value, context))
            {
                _currentContext.Value = GetActiveAncestor(
                    context.ParentContext);
            }
        }

        private DiagnosticTraceContext? GetCurrentContext()
        {
            var context = _currentContext.Value;

            while (context != null && !context.IsActive)
            {
                context = context.ParentContext;
            }

            if (!ReferenceEquals(_currentContext.Value, context))
            {
                _currentContext.Value = context;
            }

            return context;
        }

        private static DiagnosticTraceContext? GetActiveAncestor(
            DiagnosticTraceContext? context)
        {
            while (context != null && !context.IsActive)
            {
                context = context.ParentContext;
            }

            return context;
        }
    }
}
