
using System.Windows.Media;
using System.Windows.Shapes;
using UnityCommander.Abstractions.Icons;
using UnityCommander.Diagnostics.Tracing;
using UnityCommander.Rendering.Icons.Strategies;

namespace UnityCommander.Rendering.Icons
{
    public sealed class IconRenderService : IIconRenderService
    {
        private readonly IIconResolver _resolver;
        private readonly IIconBrushResolver _brushResolver;
        private readonly IIconRenderStrategyResolver _strategyResolver;
        private readonly IDiagnosticTrace _trace;

        private readonly Dictionary<string, IconRenderResult?> _cache = new();

        public IconRenderService(
            IIconResolver resolver,
            IIconBrushResolver brushResolver,
            IIconRenderStrategyResolver strategyResolver,
            IDiagnosticTrace trace)
        {
            _resolver = resolver;
            _brushResolver = brushResolver;
            _strategyResolver = strategyResolver;
            _trace = trace;
        }

        public bool TryGet(
            string key,
            out IconRenderResult result)
        {
            result = default!;

            using IDiagnosticTraceScope scope = _trace.Begin(
                "icon.render.service",
                "TryGet",
                DiagnosticTraceData.Of(
                    ("key", key)));

            try
            {
                if (TryGetFromCache(key, out result))
                {
                    scope.Complete(
                        DiagnosticTraceData.Of(
                            ("cacheHit", true)));

                    return true;
                }

                if (!TryResolveIcon(key, out RuntimeIcon icon))
                {
                    scope.Complete(
                        DiagnosticTraceData.Of(
                            ("resolved", false)));

                    return false;
                }

                var defaultColor = ResolveDefaultColor(icon);
                var strategy = ResolveStrategy(icon);

                result = strategy.Render(icon, defaultColor);

                StoreInCache(key, result);

                scope.Complete(
                    DiagnosticTraceData.Of(
                        ("resolved", true),
                        ("layerCount", result.Layers.Count)));

                return true;
            }
            catch (Exception exception)
            {
                scope.Fail(exception);
                throw;
            }
        }

        private bool TryGetFromCache(
            string key,
            out IconRenderResult result)
        {
            if (_cache.TryGetValue(key, out result))
            {
                _trace.Write(
                    "icon.render.service",
                    "cache.hit",
                    DiagnosticTraceData.Of(
                        ("key", key)));

                return true;
            }

            _trace.Write(
                "icon.render.service",
                "cache.miss",
                DiagnosticTraceData.Of(
                    ("key", key)));

            return false;
        }

        private bool TryResolveIcon(
        string key,
        out RuntimeIcon icon)
        {
            RuntimeIcon resolvedIcon = default!;

            bool found = TraceOperation(
                "icon.resolver",
                "TryResolve",
                () => _resolver.TryResolve(key, out resolvedIcon),

                inputData: DiagnosticTraceData.Of(
                    ("key", (object?)key)),

                resultData: found => DiagnosticTraceData.Of(
                    ("found", (object?)found),
                    ("iconKey", resolvedIcon?.Key),
                    ("iconType", resolvedIcon?.IconType),
                    ("layerCount", resolvedIcon?.Layers.Count ?? 0)));

            icon = resolvedIcon;

            return found;
        }

        private Brush? ResolveDefaultColor(
            RuntimeIcon icon)
        {
            return TraceOperation(
                "icon.brush",
                "Resolve",
                () => _brushResolver.Resolve(icon.Key?.ToLower()),

                inputData: DiagnosticTraceData.Of(
                    ("iconKey", icon.Key)),

                resultData: brush => DiagnosticTraceData.Of(
                    ("brushType", brush?.GetType().Name),
                    ("color", brush is SolidColorBrush solidBrush
                        ? solidBrush.Color.ToString()
                        : null),
                    ("opacity", brush?.Opacity)));
        }

        private IIconRenderStrategy ResolveStrategy(
            RuntimeIcon icon)
        {
            return TraceOperation(
                "icon.strategy",
                "Resolve",
                () => _strategyResolver.Resolve(icon.IconType),

                inputData: DiagnosticTraceData.Of(
                    ("iconType", icon.IconType)),

                resultData: strategy => DiagnosticTraceData.Of(
                    ("strategy", strategy.GetType().FullName),
                    ("iconType", icon.IconType)));
        }

        private void StoreInCache(
            string key,
            IconRenderResult result)
        {
            _cache[key] = result;

            _trace.Write(
                "icon.render.service",
                "cache.store",
                DiagnosticTraceData.Of(
                    ("key", key)));
        }

        public Path GetPath(string key)
        {
            if (!TryGet(key, out var result))
                return new Path();

            return CreatePath(result);
        }

        public Path CreatePath(IconRenderResult result)
        {
            var firstLayer = result.Layers[0];

            var geometry = new GeometryGroup();

            foreach (var layer in result.Layers)
            {
                geometry.Children.Add(layer.Geometry);
            }

            return new Path
            {
                Data = geometry,

                Fill = firstLayer.Fill,
                Stroke = firstLayer.Stroke,
                StrokeThickness = firstLayer.StrokeWidth ?? 1
            };
        }

        private T TraceOperation<T>(
            string source,
            string operation,
            Func<T> action,
            IReadOnlyDictionary<string, object?>? inputData = null,
            Func<T, IReadOnlyDictionary<string, object?>?>? resultData = null)
        {
            using IDiagnosticTraceScope scope = _trace.Begin(
                source,
                operation,
                inputData);

            try
            {
                T result = action();

                scope.Complete(resultData?.Invoke(result));

                return result;
            }
            catch (Exception exception)
            {
                scope.Fail(exception);
                throw;
            }
        }
    }
}
