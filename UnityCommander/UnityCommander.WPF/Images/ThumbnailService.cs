
using System.Collections.Concurrent;
using System.IO;
using UnityCommander.Abstractions.Thumbnail;

namespace UnityCommander.WPF.Images
{
    public sealed class ThumbnailService : IThumbnailService
    {
        private readonly IReadOnlyList<IThumbnailProvider> _providers;
        private readonly IReadOnlyList<IThumbnailCache> _caches;

        private readonly ConcurrentDictionary<
            string,
            Lazy<Task<ThumbnailResult?>>> _inflight = new();

        public ThumbnailService(
            IEnumerable<IThumbnailProvider> providers,
            IEnumerable<IThumbnailCache> caches)
        {
            _providers = providers
                .OrderByDescending(x => x.Priority)
                .ToList();

            _caches = caches
                .OrderByDescending(x => x.Priority)
                .ToList();
        }

        public async Task<ThumbnailResult?> GetAsync(
            string path,
            ThumbnailRequest request,
            CancellationToken token)
        {
            if (string.IsNullOrWhiteSpace(path))
                return null;

            foreach (var provider in _providers)
            {
                if (!provider.CanProvide(path))
                    continue;

                var providerKey =
                    await provider.GetCacheKeyAsync(
                        path,
                        request,
                        token);

                if (string.IsNullOrWhiteSpace(providerKey))
                    continue;

                var cacheKey = BuildCacheKey(
                    provider.Id,
                    providerKey,
                    request.Size);

                foreach (var cache in _caches)
                {
                    var cached = await cache.GetAsync(
                        cacheKey,
                        token);

                    if (cached is not null)
                        return cached;
                }

                var inflightKey = BuildInflightKey(
                    provider.Id,
                    path,
                    request.Size);

                var lazy = _inflight.GetOrAdd(
                    inflightKey,
                    _ => new Lazy<Task<ThumbnailResult?>>(
                        () => LoadAsync(
                            provider,
                            path,
                            request,
                            cacheKey),
                        LazyThreadSafetyMode.ExecutionAndPublication));

                try
                {
                    return await lazy.Value.WaitAsync(token);
                }
                finally
                {
                    if (lazy.IsValueCreated &&
                        lazy.Value.IsCompleted)
                    {
                        _inflight.TryRemove(
                            new KeyValuePair<
                                string,
                                Lazy<Task<ThumbnailResult?>>>(
                                inflightKey,
                                lazy));
                    }
                }
            }

            return null;
        }

        private async Task<ThumbnailResult?> LoadAsync(
            IThumbnailProvider provider,
            string path,
            ThumbnailRequest request,
            string cacheKey)
        {
            // Не используем cancellation вызывающего UI.
            // Если один visual ушёл из viewport, общую генерацию
            // всё равно может использовать другой consumer.
            var result = await provider.GetAsync(
                path,
                request,
                CancellationToken.None);

            if (result is null)
                return null;

            foreach (var cache in _caches)
            {
                try
                {
                    await cache.SetAsync(
                        cacheKey,
                        result,
                        CancellationToken.None);
                }
                catch
                {
                    // Cache failure не должен ломать thumbnail pipeline.
                }
            }

            return result;
        }

        private static string BuildCacheKey(
            string providerId,
            string providerKey,
            int size)
        {
            return $"{providerId}|{size}|{providerKey}";
        }

        private static string BuildInflightKey(
            string providerId,
            string path,
            int size)
        {
            return $"{providerId}|{size}|{NormalizePath(path)}";
        }

        private static string NormalizePath(string path)
        {
            return Path.GetFullPath(path)
                .TrimEnd(
                    Path.DirectorySeparatorChar,
                    Path.AltDirectorySeparatorChar)
                .ToUpperInvariant();
        }
    }
}
