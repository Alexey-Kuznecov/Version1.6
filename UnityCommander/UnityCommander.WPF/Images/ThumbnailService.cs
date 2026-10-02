
using System.Collections.Concurrent;
using UnityCommander.Abstractions.Thumbnail;
using UnityCommander.Diagnostics.Tracing;
using Path = System.IO.Path;

namespace UnityCommander.WPF.Images
{
    public sealed class ThumbnailService : IThumbnailService
    {
        private readonly IReadOnlyList<IThumbnailProvider> _providers;
        private readonly IReadOnlyList<IThumbnailCache> _caches;

        private readonly ConcurrentDictionary<
            string,
            Lazy<Task<ThumbnailResult?>>> _inflight = new();
        private IDiagnosticTrace _trace;

        public ThumbnailService(
            IEnumerable<IThumbnailProvider> providers,
            IEnumerable<IThumbnailCache> caches, 
            IDiagnosticTrace diagnosticTrace)
        {
            _trace = diagnosticTrace;

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
                var canProvide = provider.CanProvide(path);

                _trace.Write(
                    "ThumbnailService",
                    "ProviderCheck",
                    new Dictionary<string, object?>
                    {
                        ["Provider"] = provider.Id,
                        ["Path"] = path,
                        ["CanProvide"] = canProvide
                    });

                if (!canProvide)
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

                    _trace.Write(
                        "ThumbnailService",
                        "CacheMiss",
                        new Dictionary<string, object?>
                        {
                            ["Provider"] = provider.Id,
                            ["Path"] = path
                        });

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
                    var result = await lazy.Value.WaitAsync(token);

                    _trace.Write(
                        "ThumbnailService",
                        "ProviderCompleted",
                        new Dictionary<string, object?>
                        {
                            ["Provider"] = provider.Id,
                            ["Path"] = path,
                            ["HasResult"] = result is not null
                        });

                    if (result is not null)
                        return result;
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

            _trace.Write(
                "ThumbnailService",
                "NoThumbnail",
                new Dictionary<string, object?>
                {
                    ["Path"] = path
                });

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
