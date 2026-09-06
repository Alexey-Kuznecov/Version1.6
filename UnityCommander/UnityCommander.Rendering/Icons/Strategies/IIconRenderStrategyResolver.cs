
using UnityCommander.Abstractions.Icons;

namespace UnityCommander.Rendering.Icons.Strategies
{
    public interface IIconRenderStrategyResolver
    {
        IIconRenderStrategy Resolve(IconType type);
    }
}
