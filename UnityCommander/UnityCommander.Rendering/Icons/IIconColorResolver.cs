
using System.Windows.Media;

namespace UnityCommander.Rendering.Icons
{
    public interface IIconColorResolver
    {
        Brush Resolve(
            IconRole role, 
            IconTone tone, 
            VisualState state);
    }
}
