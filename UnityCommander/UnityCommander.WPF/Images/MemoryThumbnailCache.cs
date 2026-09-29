
using System.Collections.Concurrent;
using UnityCommander.Abstractions.Thumbnail;

namespace UnityCommander.WPF.Images
{
    public sealed class MemoryThumbnailCache : IThumbnailCache
    {
        private readonly ConcurrentDictionary<string, ThumbnailResult> _items =
            new(StringComparer.Ordinal);

        private readonly ConcurrentQueue<string> _order = new();

        private readonly int _maxItems;

        public int Priority => 100;

        public MemoryThumbnailCache(int maxItems = 256)
        {
            if (maxItems <= 0)
                throw new ArgumentOutOfRangeException(nameof(maxItems));

            _maxItems = maxItems;
        }

        public Task<ThumbnailResult?> GetAsync(
            string key,
            CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            return Task.FromResult(
                _items.TryGetValue(key, out var result)
                    ? result
                    : null);
        }

        public Task SetAsync(
            string key,
            ThumbnailResult value,
            CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            _items[key] = value;
            _order.Enqueue(key);

            Trim();

            return Task.CompletedTask;
        }

        private void Trim()
        {
            while (_items.Count > _maxItems &&
                   _order.TryDequeue(out var key))
            {
                _items.TryRemove(key, out _);
            }
        }
    }
}
