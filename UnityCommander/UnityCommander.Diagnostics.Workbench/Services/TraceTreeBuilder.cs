
using UnityCommander.Diagnostics.Workbench.Models;

namespace UnityCommander.Diagnostics.Workbench.Services
{
    public static class TraceTreeBuilder
    {
        private sealed class SpanGroup
        {
            public required string TraceId { get; init; }

            public required string SpanId { get; init; }

            public string? ParentSpanId { get; set; }

            public List<TraceEventItem> Events { get; } = new();

            public List<SpanGroup> Children { get; } = new();

            public DateTime Timestamp =>
                Events
                    .FirstOrDefault(IsBeginEvent)?.Timestamp
                    ?? Events.Min(item => item.Timestamp);

            public string Title
            {
                get
                {
                    TraceEventItem? begin = Events.FirstOrDefault(IsBeginEvent);

                    TraceEventItem source = begin
                        ?? Events.OrderBy(item => item.Timestamp).First();

                    return GetOperationName(source.EventName);
                }
            }

            public TraceEventItem RepresentativeEvent =>
                Events
                    .LastOrDefault(IsCompletionEvent)
                    ?? Events.OrderBy(item => item.Timestamp).Last();

            public double? DurationMilliseconds =>
                Events
                    .Where(item => item.DurationMilliseconds > 0)
                    .Select(item => item.DurationMilliseconds)
                    .DefaultIfEmpty(0)
                    .Max();

            public bool IsFailed =>
                Events.Any(item => item.EventName.EndsWith(
                    ".failed",
                    StringComparison.OrdinalIgnoreCase));
        }

        public static IReadOnlyList<TraceTreeRow> Build(
            IEnumerable<TraceEventItem> events)
        {
            var allEvents = events
                .OrderBy(item => item.Timestamp)
                .ToList();

            var groups = allEvents
                .Where(item => !string.IsNullOrWhiteSpace(item.SpanId))
                .GroupBy(item => (item.TraceId, item.SpanId!))
                .ToDictionary(
                    group => group.Key,
                    group =>
                    {
                        var span = new SpanGroup
                        {
                            TraceId = group.Key.TraceId,
                            SpanId = group.Key.Item2,
                            ParentSpanId = group.First().ParentSpanId
                        };

                        span.Events.AddRange(group);

                        return span;
                    });

            var roots = new List<SpanGroup>();

            foreach (SpanGroup span in groups.Values)
            {
                if (span.ParentSpanId is not null &&
                    groups.TryGetValue(
                        (span.TraceId, span.ParentSpanId),
                        out SpanGroup? parent))
                {
                    parent.Children.Add(span);
                }
                else
                {
                    roots.Add(span);
                }
            }

            var result = new List<TraceTreeRow>();

            foreach (SpanGroup root in roots.OrderBy(item => item.Timestamp))
            {
                result.Add(BuildSpanRow(root, 0));
            }

            // События без Span отображаем отдельными корневыми строками.
            foreach (TraceEventItem item in allEvents.Where(
                         item => string.IsNullOrWhiteSpace(item.SpanId)))
            {
                result.Add(BuildEventRow(item, 0));
            }

            return result
                .OrderBy(item => item.Timestamp)
                .ToList();
        }

        private static TraceTreeRow BuildSpanRow(
            SpanGroup span,
            int depth)
        {
            var row = new TraceTreeRow(
                title: span.Title,
                source: span.RepresentativeEvent.Source,
                timestamp: span.Timestamp,
                durationMilliseconds: span.DurationMilliseconds,
                indentLevel: depth,
                representativeEvent: span.RepresentativeEvent,
                isFailed: span.IsFailed,
                searchEvents: span.Events,
                nodeKey: $"span:{span.TraceId}:{span.SpanId}");

            var children = new List<TraceTreeRow>();

            // Все события, принадлежащие этой операции.
            foreach (TraceEventItem item in span.Events)
            {
                children.Add(BuildEventRow(item, depth + 1));
            }

            // Вложенные операции.
            foreach (SpanGroup child in span.Children)
            {
                children.Add(BuildSpanRow(child, depth + 1));
            }

            // События и вложенные операции располагаем по времени.
            foreach (TraceTreeRow child in children.OrderBy(item => item.Timestamp))
            {
                row.Children.Add(child);
            }

            return row;
        }

        private static TraceTreeRow BuildEventRow(
            TraceEventItem item,
            int depth)
        {
            bool isFailed = item.EventName.EndsWith(
                ".failed",
                StringComparison.OrdinalIgnoreCase);

            return new TraceTreeRow(
                title: item.EventName,
                source: item.Source,
                timestamp: item.Timestamp,
                durationMilliseconds: item.DurationMilliseconds,
                indentLevel: depth,
                representativeEvent: item,
                isFailed: isFailed,
                searchEvents: new[] { item },
                isEvent: true);
        }

        private static bool IsBeginEvent(TraceEventItem item)
        {
            return item.EventName.EndsWith(
                ".begin",
                StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsCompletionEvent(TraceEventItem item)
        {
            return item.EventName.EndsWith(
                       ".complete",
                       StringComparison.OrdinalIgnoreCase)
                   || item.EventName.EndsWith(
                       ".failed",
                       StringComparison.OrdinalIgnoreCase);
        }

        private static string GetOperationName(string eventName)
        {
            foreach (string suffix in new[] { ".begin", ".complete", ".failed" })
            {
                if (eventName.EndsWith(
                    suffix,
                    StringComparison.OrdinalIgnoreCase))
                {
                    return eventName[..^suffix.Length];
                }
            }

            return eventName;
        }
    }
}
