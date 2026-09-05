
using System.Windows.Media;

namespace UnityCommander.Rendering.Icons
{
    public interface IIconBrushResolver
    {
        Brush Resolve(string? key);
    }
}
