namespace UnityCommander.Diagnostics.Tracing
{
    public sealed class DiagnosticTraceScope : IDiagnosticTraceScope
    {
        private readonly DiagnosticTrace _trace;
        private readonly string _source;

        private int _depth;
        private bool _completed;

        public string Id { get; }

        public DiagnosticTraceScope(
            DiagnosticTrace trace,
            string source,
            string operation,
            IReadOnlyDictionary<string, object?>? data)
        {
            _trace = trace;
            _source = source;

            Id = Guid.NewGuid().ToString("N");

            _trace.Write(
                Id,
                _source,
                $"{operation}.begin",
                _depth,
                data);
        }

        public void Write(
            string eventName,
            IReadOnlyDictionary<string, object?>? data = null)
        {
            if (_completed)
                return;

            _trace.Write(
                Id,
                _source,
                eventName,
                _depth,
                data);
        }

        public void Complete(
            IReadOnlyDictionary<string, object?>? data = null)
        {
            if (_completed)
                return;

            _completed = true;

            _trace.Write(
                Id,
                _source,
                "complete",
                _depth,
                data);
        }

        public void Fail(
            Exception exception,
            IReadOnlyDictionary<string, object?>? data = null)
        {
            if (_completed)
                return;

            _completed = true;

            var result = new Dictionary<string, object?>
            {
                ["exception"] = exception.GetType().FullName,
                ["message"] = exception.Message
            };

            if (data != null)
            {
                foreach (var pair in data)
                    result[pair.Key] = pair.Value;
            }

            _trace.Write(
                Id,
                _source,
                "failed",
                _depth,
                result);
        }

        public void Dispose()
        {
            if (!_completed)
                Complete();
        }
    }
}