
using System.Collections.Concurrent;
using UnityCommander.Abstractions.Thumbnail;
using UnityCommander.Diagnostics.Tracing;

namespace UnityCommander.WPF.Images
{
    public sealed class MemoryThumbnailCache : IThumbnailCache
    {
        private readonly ConcurrentDictionary<string, ThumbnailResult> _items =
            new(StringComparer.Ordinal);

        private readonly ConcurrentQueue<string> _order = new();

        private readonly IDiagnosticTrace _trace;

        private readonly int _maxItems;

        public int Priority => 100;

        public MemoryThumbnailCache(
            IDiagnosticTrace trace,
            int maxItems = 256)
        {
            if (maxItems <= 0)
                throw new ArgumentOutOfRangeException(nameof(maxItems));

            _trace = trace;
            _maxItems = maxItems;
        }

        public Task<ThumbnailResult?> GetAsync(
            string key,
            CancellationToken token)
        {
            using var trace = _trace.Begin(
                "thumbnail.cache.memory",
                "get",
                DiagnosticTraceData.Of(
                    ("key", key)));

            try
            {
                token.ThrowIfCancellationRequested();

                bool found = _items.TryGetValue(
                    key,
                    out ThumbnailResult? result);

                trace.Complete(
                    DiagnosticTraceData.Of(
                        ("hit", found),
                        ("itemCount", _items.Count),
                        ("width", result?.Width),
                        ("height", result?.Height),
                        ("pngBytes", result?.PngData.Length)));

                return Task.FromResult(result);
            }
            catch (Exception exception)
            {
                trace.Fail(exception);
                throw;
            }
        }

        public Task SetAsync(
            string key,
            ThumbnailResult value,
            CancellationToken token)
        {
            using var trace = _trace.Begin(
                "thumbnail.cache.memory",
                "set",
                DiagnosticTraceData.Of(
                    ("key", key),
                    ("width", value.Width),
                    ("height", value.Height),
                    ("pngBytes", value.PngData.Length)));

            try
            {
                token.ThrowIfCancellationRequested();

                _items[key] = value;
                _order.Enqueue(key);

                int removedItems = Trim();

                trace.Complete(
                    DiagnosticTraceData.Of(
                        ("itemCount", _items.Count),
                        ("maxItems", _maxItems),
                        ("removedItems", removedItems)));

                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                trace.Fail(exception);
                throw;
            }
        }

        private int Trim()
        {
            int removedItems = 0;

            while (_items.Count > _maxItems &&
                   _order.TryDequeue(out string? key))
            {
                if (_items.TryRemove(key, out _))
                {
                    removedItems++;
                }
            }

            return removedItems;
        }
    }
}
