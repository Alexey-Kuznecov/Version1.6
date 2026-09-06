
using UnityCommander.Rendering.Icons;

namespace UnityCommander.Rendering.Icons.Services
{
    public interface IIconRenderNormalizer
    {
        IconRenderResult Normalize(
            IReadOnlyList<IconRenderLayer> layers);
    }
}
