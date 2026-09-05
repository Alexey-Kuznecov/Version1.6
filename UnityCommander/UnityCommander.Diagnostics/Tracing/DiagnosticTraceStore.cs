
namespace UnityCommander.Diagnostics.Tracing
{
    public sealed class DiagnosticTraceStore : IDiagnosticTraceStore
    {
        private readonly object _sync = new();

        private readonly Queue<DiagnosticTraceEvent> _entries = new();

        private readonly int _capacity;

        public DiagnosticTraceStore(int capacity = 10_000)
        {
            if (capacity <= 0)
                throw new ArgumentOutOfRangeException(nameof(capacity));

            _capacity = capacity;
        }

        public void Add(DiagnosticTraceEvent entry)
        {
            ArgumentNullException.ThrowIfNull(entry);

            lock (_sync)
            {
                _entries.Enqueue(entry);

                while (_entries.Count > _capacity)
                    _entries.Dequeue();
            }
        }

        public IReadOnlyList<DiagnosticTraceEvent> GetSnapshot()
        {
            lock (_sync)
            {
                return _entries.ToList();
            }
        }

        public IReadOnlyList<DiagnosticTraceEvent> Query(
            DiagnosticTraceQuery query)
        {
            ArgumentNullException.ThrowIfNull(query);

            lock (_sync)
            {
                IEnumerable<DiagnosticTraceEvent> result = _entries;

                if (!string.IsNullOrWhiteSpace(query.Source))
                {
                    result = result.Where(x =>
                        string.Equals(
                            x.Source,
                            query.Source,
                            StringComparison.OrdinalIgnoreCase));
                }

                if (!string.IsNullOrWhiteSpace(query.EventName))
                {
                    result = result.Where(x =>
                        string.Equals(
                            x.EventName,
                            query.EventName,
                            StringComparison.OrdinalIgnoreCase));
                }

                if (!string.IsNullOrWhiteSpace(query.TraceId))
                {
                    result = result.Where(x =>
                        string.Equals(
                            x.TraceId,
                            query.TraceId,
                            StringComparison.OrdinalIgnoreCase));
                }

                if (query.From.HasValue)
                    result = result.Where(x => x.Timestamp >= query.From.Value);

                if (query.To.HasValue)
                    result = result.Where(x => x.Timestamp <= query.To.Value);

                if (query.Data != null)
                {
                    result = result.Where(entry =>
                        query.Data.All(filter =>
                            entry.Data.TryGetValue(filter.Key, out var value) &&
                            Equals(value, filter.Value)));
                }

                result = result.OrderBy(x => x.Timestamp);

                if (query.Limit is > 0)
                    result = result.TakeLast(query.Limit.Value);

                return result.ToList();
            }
        }

        public void Clear()
        {
            lock (_sync)
            {
                _entries.Clear();
            }
        }
    }
}
