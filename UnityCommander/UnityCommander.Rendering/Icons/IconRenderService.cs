
using System.Windows.Shapes;
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

        public bool TryGet(string key, out IconRenderResult result)
        {
            if (_cache.TryGetValue(key, out result))
                return true;

            if (!_resolver.TryResolve(key, out var icon))
                return false;

            var defaultColor =
                _brushResolver.Resolve(icon.Key?.ToLower());

            var strategy =
                _strategyResolver.Resolve(icon.IconType);

            result = strategy.Render(icon, defaultColor);

            _cache[key] = result;

            return true;
        }

        public Path GetPath(string key)
        {
            if (!TryGet(key, out var result))
                return new Path();

            return CreatePath(result);
        }

        public Path CreatePath(IconRenderResult result)
        {
            return new Path
            {
                //Data = result.Geometry,

                //Fill = result.Brush,
                //Stroke = result.Stroke,
                //StrokeThickness = result.StrokeWidth ?? 1,

                //Width = result.Size,
                //Height = result.Size,
                //Stretch = Stretch.Uniform
            };
        }
    }
}
