
using System.Windows.Media;
using UnityCommander.Abstractions.Icons;

namespace UnityCommander.Rendering.Icons
{
    public interface IIconRenderStrategy
    {
        IconType Type { get; }

        IconRenderResult Render(
            RuntimeIcon icon,
            Brush defaultColor);
    }
}
