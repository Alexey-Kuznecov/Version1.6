
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
            using var scope = _trace.Begin(
                "thumbnail.service",
                "GetAsync",
                DiagnosticTraceData.Of(
                    ("path", path),
                    ("extension", Path.GetExtension(path)),
                    ("requestedSize", request.Size),
                    ("providerCount", _providers.Count),
                    ("cacheCount", _caches.Count)));

            try
            {
                if (string.IsNullOrWhiteSpace(path))
                {
                    scope.Complete(
                        DiagnosticTraceData.Of(
                            ("hasThumbnail", false),
                            ("reason", "EmptyPath")));

                    return null;
                }

                foreach (var provider in _providers)
                {
                    var canProvide = provider.CanProvide(path);

                    _trace.Write(
                        "thumbnail.provider",
                        "provider.check",
                        DiagnosticTraceData.Of(
                            ("provider", provider.Id),
                            ("path", path),
                            ("canProvide", canProvide)));

                    if (!canProvide)
                        continue;

                    string? providerKey;

                    using (var keyScope = _trace.Begin(
                        "thumbnail.provider",
                        "GetCacheKeyAsync",
                        DiagnosticTraceData.Of(
                            ("provider", provider.Id),
                            ("path", path),
                            ("requestedSize", request.Size))))
                    {
                        try
                        {
                            providerKey = await provider.GetCacheKeyAsync(
                                path,
                                request,
                                token);

                            keyScope.Complete(
                                DiagnosticTraceData.Of(
                                    ("hasKey", !string.IsNullOrWhiteSpace(providerKey)),
                                    ("keyLength", providerKey?.Length ?? 0)));
                        }
                        catch (Exception exception)
                        {
                            keyScope.Fail(exception);
                            throw;
                        }
                    }

                    if (string.IsNullOrWhiteSpace(providerKey))
                        continue;

                    var cacheKey = BuildCacheKey(
                        provider.Id,
                        providerKey,
                        request.Size);

                    foreach (var cache in _caches)
                    {
                        using var cacheScope = _trace.Begin(
                            "thumbnail.cache",
                            "GetAsync",
                            DiagnosticTraceData.Of(
                                ("cache", cache.GetType().Name),
                                ("provider", provider.Id),
                                ("cacheKey", cacheKey),
                                ("requestedSize", request.Size)));

                        try
                        {
                            var cached = await cache.GetAsync(
                                cacheKey,
                                token);

                            cacheScope.Complete(
                                DiagnosticTraceData.Of(
                                    ("hit", cached is not null),
                                    ("width", cached?.Width),
                                    ("height", cached?.Height),
                                    ("bytes", cached?.PngData.Length ?? 0)));

                            if (cached is not null)
                            {
                                scope.Complete(
                                    DiagnosticTraceData.Of(
                                        ("hasThumbnail", true),
                                        ("resultSource", "Cache"),
                                        ("provider", provider.Id),
                                        ("width", cached.Width),
                                        ("height", cached.Height),
                                        ("bytes", cached.PngData.Length)));

                                return cached;
                            }
                        }
                        catch (Exception exception)
                        {
                            cacheScope.Fail(exception);
                            throw;
                        }
                    }

                    var inflightKey = BuildInflightKey(
                        provider.Id,
                        path,
                        request.Size);

                    var candidate = new Lazy<Task<ThumbnailResult?>>(
                        () => LoadAsync(
                            provider,
                            path,
                            request,
                            cacheKey),
                        LazyThreadSafetyMode.ExecutionAndPublication);

                    var lazy = _inflight.GetOrAdd(
                        inflightKey,
                        candidate);

                    var joinedExisting = !ReferenceEquals(
                        candidate,
                        lazy);

                    try
                    {
                        using var waitScope = _trace.Begin(
                            "thumbnail.service",
                            "GetOrJoinGeneration",
                            DiagnosticTraceData.Of(
                                ("provider", provider.Id),
                                ("path", path),
                                ("requestedSize", request.Size),
                                ("joinedExisting", joinedExisting)));

                        try
                        {
                            var result = await lazy.Value.WaitAsync(token);

                            waitScope.Complete(
                                DiagnosticTraceData.Of(
                                    ("hasResult", result is not null),
                                    ("width", result?.Width),
                                    ("height", result?.Height),
                                    ("bytes", result?.PngData.Length ?? 0)));

                            if (result is not null)
                            {
                                scope.Complete(
                                    DiagnosticTraceData.Of(
                                        ("hasThumbnail", true),
                                        ("resultSource", "Provider"),
                                        ("provider", provider.Id),
                                        ("joinedExisting", joinedExisting),
                                        ("width", result.Width),
                                        ("height", result.Height),
                                        ("bytes", result.PngData.Length)));

                                return result;
                            }
                        }
                        catch (Exception exception)
                        {
                            waitScope.Fail(
                                exception,
                                DiagnosticTraceData.Of(
                                    ("cancelled", exception is OperationCanceledException)));

                            throw;
                        }
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

                scope.Complete(
                    DiagnosticTraceData.Of(
                        ("hasThumbnail", false),
                        ("reason", "NoProviderProducedResult")));

                return null;
            }
            catch (Exception exception)
            {
                scope.Fail(
                    exception,
                    DiagnosticTraceData.Of(
                        ("cancelled", exception is OperationCanceledException)));

                throw;
            }
        }

        private async Task<ThumbnailResult?> LoadAsync(
            IThumbnailProvider provider,
            string path,
            ThumbnailRequest request,
            string cacheKey)
        {
            using var scope = _trace.Begin(
                "thumbnail.service",
                "LoadAsync",
                DiagnosticTraceData.Of(
                    ("provider", provider.Id),
                    ("path", path),
                    ("requestedSize", request.Size),
                    ("cacheKey", cacheKey)));

            try
            {
                ThumbnailResult? result;

                using (var providerScope = _trace.Begin(
                    "thumbnail.provider",
                    "GetAsync",
                    DiagnosticTraceData.Of(
                        ("provider", provider.Id),
                        ("path", path),
                        ("requestedSize", request.Size))))
                {
                    try
                    {
                        // Не используем cancellation вызывающего UI.
                        // Общую генерацию могут использовать другие consumers.
                        result = await provider.GetAsync(
                            path,
                            request,
                            CancellationToken.None);

                        providerScope.Complete(
                            DiagnosticTraceData.Of(
                                ("hasResult", result is not null),
                                ("width", result?.Width),
                                ("height", result?.Height),
                                ("bytes", result?.PngData.Length ?? 0)));
                    }
                    catch (Exception exception)
                    {
                        providerScope.Fail(exception);
                        throw;
                    }
                }

                if (result is null)
                {
                    scope.Complete(
                        DiagnosticTraceData.Of(
                            ("hasThumbnail", false),
                            ("reason", "ProviderReturnedNull")));

                    return null;
                }

                foreach (var cache in _caches)
                {
                    using var cacheScope = _trace.Begin(
                        "thumbnail.cache",
                        "SetAsync",
                        DiagnosticTraceData.Of(
                            ("cache", cache.GetType().Name),
                            ("provider", provider.Id),
                            ("cacheKey", cacheKey),
                            ("width", result.Width),
                            ("height", result.Height),
                            ("bytes", result.PngData.Length)));

                    try
                    {
                        await cache.SetAsync(
                            cacheKey,
                            result,
                            CancellationToken.None);

                        cacheScope.Complete(
                            DiagnosticTraceData.Of(
                                ("stored", true)));
                    }
                    catch (Exception exception)
                    {
                        // Ошибка записи кэша не должна ломать
                        // генерацию миниатюры.
                        cacheScope.Fail(
                            exception,
                            DiagnosticTraceData.Of(
                                ("stored", false),
                                ("ignored", true)));
                    }
                }

                scope.Complete(
                    DiagnosticTraceData.Of(
                        ("hasThumbnail", true),
                        ("provider", provider.Id),
                        ("width", result.Width),
                        ("height", result.Height),
                        ("bytes", result.PngData.Length)));

                return result;
            }
            catch (Exception exception)
            {
                scope.Fail(exception);
                throw;
            }
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
