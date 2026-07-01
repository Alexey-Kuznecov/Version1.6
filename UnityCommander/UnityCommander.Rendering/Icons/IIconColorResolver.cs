
using System.Windows.Media;
using UnityCommander.Abstractions;

namespace UnityCommander.Rendering.Icons
{
    public interface IIconColorResolver
    {
        Brush Resolve(
             IconKind kind,
             IconRole role,
             IconTone tone,
             VisualState state);
    }
}
