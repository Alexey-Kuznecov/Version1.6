
using UnityCommander.Abstractions.Icons;

namespace UnityCommander.Rendering.Icons.Strategies
{
    public sealed class IconRenderStrategyResolver
      : IIconRenderStrategyResolver
    {
        private readonly FilledIconRenderStrategy _filled;
        private readonly StrokeIconRenderStrategy _stroke;
        private readonly LayeredIconRenderStrategy _layered;

        public IconRenderStrategyResolver(
            FilledIconRenderStrategy filled,
            StrokeIconRenderStrategy stroke,
            LayeredIconRenderStrategy layered)
        {
            _filled = filled;
            _stroke = stroke;
            _layered = layered;
        }

        public IIconRenderStrategy Resolve(IconType type)
        {
            return type switch
            {
                IconType.Filled => _filled,
                IconType.Stroke => _stroke,
                IconType.Layered => _layered,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }
    }
}
